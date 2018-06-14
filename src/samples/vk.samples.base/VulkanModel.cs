// This code has been adapted from the "Vulkan" C++ example repository, by Sascha Willems: https://github.com/SaschaWillems/Vulkan
// It is a direct translation from the original C++ code and style, with as little transformation as possible.

// Original file: base/VulkanModel.hpp, 

/*
* Vulkan Example base class
*
* Copyright (C) 2016 by Sascha Willems - www.saschawillems.de
*
* This code is licensed under the MIT license (MIT) (http://opensource.org/licenses/MIT)
*/

using Assimp;
using System;
using System.Diagnostics;
using System.Numerics;
using Vulkan;
using System.Collections.Generic;
using static Vulkan.VulkanNative;

namespace Vk.Samples
{
    public enum VertexComponent
    {
        VERTEX_COMPONENT_POSITION = 0x0,
        VERTEX_COMPONENT_NORMAL = 0x1,
        VERTEX_COMPONENT_COLOR = 0x2,
        VERTEX_COMPONENT_UV = 0x3,
        VERTEX_COMPONENT_TANGENT = 0x4,
        VERTEX_COMPONENT_BITANGENT = 0x5,
        VERTEX_COMPONENT_DUMMY_FLOAT = 0x6,
        VERTEX_COMPONENT_DUMMY_VEC4 = 0x7
    }

    public class vksVertexLayout
    {
        public List<VertexComponent> Components { get; } = new List<VertexComponent>();

        public vksVertexLayout(params VertexComponent[] components) { Components = new List<VertexComponent>(components); }

        public uint stride() => GetStride();
        public uint GetStride()
        {
            uint result = 0;
            foreach (var component in Components)
            {
                switch (component)
                {
                    case VertexComponent.VERTEX_COMPONENT_UV:
                        result += 2 * sizeof(float);
                        break;
                    case VertexComponent.VERTEX_COMPONENT_DUMMY_FLOAT:
                        result += sizeof(float);
                        break;
                    case VertexComponent.VERTEX_COMPONENT_DUMMY_VEC4:
                        result += 4 * sizeof(float);
                        break;
                    case VertexComponent.VERTEX_COMPONENT_POSITION:
                    case VertexComponent.VERTEX_COMPONENT_NORMAL:
                    case VertexComponent.VERTEX_COMPONENT_COLOR:
                    case VertexComponent.VERTEX_COMPONENT_TANGENT:
                    case VertexComponent.VERTEX_COMPONENT_BITANGENT:
                        result += 3 * sizeof(float);
                        break;
                    default:
                        throw new InvalidOperationException();
                }
            }

            return result;
        }
    }

    public struct vksModelCreateInfo
    {
        public Vector3 Center;
        public Vector3 Scale;
        public Vector2 UVScale;

        public vksModelCreateInfo(Vector3 scale, Vector2 uvScale, Vector3 center)
        {
            Center = center;
            Scale = scale;
            UVScale = uvScale;
        }

        public vksModelCreateInfo(float scale, float uvScale, float center)
        {
            Center = new Vector3(center);
            Scale = new Vector3(scale);
            UVScale = new Vector2(uvScale);
        }
    }

    public unsafe class vksModel
    {
        private const PostProcessSteps DefaultPostProcessSteps = 
            PostProcessSteps.FlipWindingOrder | PostProcessSteps.Triangulate | PostProcessSteps.PreTransformVertices 
            | PostProcessSteps.CalculateTangentSpace | PostProcessSteps.GenerateSmoothNormals;
        public VkDevice device;
        public vksBuffer vertices = new vksBuffer();
        public vksBuffer indices = new vksBuffer();
        public uint indexCount = 0;
        public uint vertexCount = 0;

        public struct ModelPart
        {
            public uint vertexBase;
            public uint vertexCount;
            public uint indexBase;
            public uint indexCount;
        }

        NativeList<ModelPart> parts = new NativeList<ModelPart>();

        public struct Dimension
        {
            public Vector3 Min;
            public Vector3 Max;
            public Vector3 Size;
            public Dimension(Vector3 min, Vector3 max) { Min = min; Max = max; Size = new Vector3(); }
        }

        public Dimension dim = new Dimension(new Vector3(float.MaxValue), new Vector3(float.MinValue));

        /** @brief Release all Vulkan resources of this model */
        public void destroy()
        {
            Debug.Assert(device.Handle != null);
            vkDestroyBuffer(device, vertices.buffer, null);
            vkFreeMemory(device, vertices.memory, null);
            if (indices.buffer.Handle != 0)
            {
                vkDestroyBuffer(device, indices.buffer, null);
                vkFreeMemory(device, indices.memory, null);
            }
        }

        /**
        * Loads a 3D model from a file into Vulkan buffers
        *
        * @param device Pointer to the Vulkan device used to generated the vertex and index buffers on
        * @param filename File to load (must be a model format supported by ASSIMP)
        * @param layout Vertex layout components (position, normals, tangents, etc.)
        * @param createInfo MeshCreateInfo structure for load time settings like scale, center, etc.
        * @param copyQueue Queue used for the memory staging copy commands (must support transfer)
        * @param (Optional) flags ASSIMP model loading flags
        */
        //public bool loadFromFile(string filename, vksVertexLayout layout, vksModelCreateInfo* createInfo, vksVulkanDevice device, VkQueue copyQueue, int flags)
        //{
        //    this.device = device.LogicalDevice;

        //    ObjFile objFile;
        //    using (var fs = File.OpenRead(filename))
        //    {
        //        objFile = new ObjParser().Parse(fs);
        //    }
        //    string mtlFilename = Path.ChangeExtension(filename, "mtl");
        //    MtlFile mtlFile;
        //    using (var fs = File.OpenRead(mtlFilename))
        //    {
        //        mtlFile = new MtlParser().Parse(fs);
        //    }

        //    if (objFile != null)
        //    {
        //        parts.Clear();
        //        parts.Resize((uint)objFile.MeshGroups.Length);

        //        Vector3 scale = new Vector3(1.0f);
        //        Vector2 uvscale = new Vector2(1.0f);
        //        Vector3 center = new Vector3(0.0f);
        //        if (createInfo != null)
        //        {
        //            scale = createInfo->Scale;
        //            uvscale = createInfo->UVScale;
        //            center = createInfo->Center;
        //        }

        //        NativeList<float> vertexBuffer = new NativeList<float>();
        //        NativeList<uint> indexBuffer = new NativeList<uint>();

        //        vertexCount = 0;
        //        indexCount = 0;

        //        // Load meshes
        //        parts.Count = (uint)objFile.MeshGroups.Length;
        //        for (uint i = 0; i < objFile.MeshGroups.Length; i++)
        //        {
        //            ConstructedMeshInfo mesh = objFile.GetMesh(objFile.MeshGroups[i]);

        //            parts[i] = new ModelPart();
        //            parts[i].vertexBase = vertexCount;
        //            parts[i].indexBase = indexCount;

        //            vertexCount += (uint)mesh.Vertices.Length;

        //            var material = mtlFile.Definitions[objFile.MeshGroups[i].Material];
        //            Vector3 pColor = material.DiffuseReflectivity;

        //            Vector3 Zero3D = Vector3.Zero;

        //            for (uint j = 0; j < mesh.Vertices.Length; j++)
        //            {
        //                VertexPositionNormalTexture vertex = mesh.Vertices[j];
        //                Vector3* pPos = &vertex.Position;
        //                Vector3* pNormal = &vertex.Normal;
        //                Vector2* pTexCoord = &vertex.TextureCoordinates;
        //                Vector3* pTangent = &Zero3D;
        //                Vector3* pBiTangent = &Zero3D;

        //                /*
        //                const aiVector3D* pPos = &(paiMesh->mVertices[j]);
        //                const aiVector3D* pNormal = &(paiMesh->mNormals[j]);
        //                const aiVector3D* pTexCoord = (paiMesh->HasTextureCoords(0)) ? &(paiMesh->mTextureCoords[0][j]) : &Zero3D;
        //                const aiVector3D* pTangent = (paiMesh->HasTangentsAndBitangents()) ? &(paiMesh->mTangents[j]) : &Zero3D;
        //                const aiVector3D* pBiTangent = (paiMesh->HasTangentsAndBitangents()) ? &(paiMesh->mBitangents[j]) : &Zero3D;
        //                */

        //                foreach (var component in layout.Components)
        //                {
        //                    switch (component)
        //                    {
        //                        case VertexComponent.VERTEX_COMPONENT_POSITION:
        //                            vertexBuffer.Add(pPos.X * scale.X + center.X);
        //                            vertexBuffer.Add(-pPos.Y * scale.Y + center.Y);
        //                            vertexBuffer.Add(pPos.Z * scale.Z + center.Z);
        //                            break;
        //                        case VertexComponent.VERTEX_COMPONENT_NORMAL:
        //                            vertexBuffer.Add(pNormal.X);
        //                            vertexBuffer.Add(-pNormal.Y);
        //                            vertexBuffer.Add(pNormal.Z);
        //                            break;
        //                        case VertexComponent.VERTEX_COMPONENT_UV:
        //                            vertexBuffer.Add(pTexCoord.X * uvscale.X);
        //                            vertexBuffer.Add(pTexCoord.Y * uvscale.Y);
        //                            break;
        //                        case VertexComponent.VERTEX_COMPONENT_COLOR:
        //                            vertexBuffer.Add(pColor.X);
        //                            vertexBuffer.Add(pColor.Y);
        //                            vertexBuffer.Add(pColor.Z);
        //                            break;
        //                        case VertexComponent.VERTEX_COMPONENT_TANGENT:
        //                            vertexBuffer.Add(pTangent.X);
        //                            vertexBuffer.Add(pTangent.Y);
        //                            vertexBuffer.Add(pTangent.Z);
        //                            break;
        //                        case VertexComponent.VERTEX_COMPONENT_BITANGENT:
        //                            vertexBuffer.Add(pBiTangent.X);
        //                            vertexBuffer.Add(pBiTangent.Y);
        //                            vertexBuffer.Add(pBiTangent.Z);
        //                            break;
        //                        // Dummy components for padding
        //                        case VertexComponent.VERTEX_COMPONENT_DUMMY_FLOAT:
        //                            vertexBuffer.Add(0.0f);
        //                            break;
        //                        case VertexComponent.VERTEX_COMPONENT_DUMMY_VEC4:
        //                            vertexBuffer.Add(0.0f);
        //                            vertexBuffer.Add(0.0f);
        //                            vertexBuffer.Add(0.0f);
        //                            vertexBuffer.Add(0.0f);
        //                            break;
        //                    };
        //                }

        //                dim.Max.X = Math.Max(pPos.X, dim.Max.X);
        //                dim.Max.Y = Math.Max(pPos.Y, dim.Max.Y);
        //                dim.Max.Z = Math.Max(pPos.Z, dim.Max.Z);

        //                dim.Min.X = Math.Min(pPos.X, dim.Min.X);
        //                dim.Min.Y = Math.Min(pPos.Y, dim.Min.Y);
        //                dim.Min.Z = Math.Min(pPos.Z, dim.Min.Z);
        //            }

        //            dim.Size = dim.Max - dim.Min;

        //            parts[i].vertexCount = (uint)mesh.Vertices.Length;

        //            uint indexBase = indexBuffer.Count;

        //            foreach (var index in mesh.Indices)
        //            {
        //                indexBuffer.Add(index);
        //            }
        //            indexCount += (uint)mesh.Indices.Length;
        //            parts[i].indexCount += (uint)mesh.Indices.Length;

        //            /*
        //            for (uint j = 0; j < paiMesh->mNumFaces; j++)
        //            {
        //                const aiFace&Face = paiMesh->mFaces[j];
        //                if (Face.mNumIndices != 3)
        //                    continue;
        //                indexBuffer.Add(indexBase + Face.mIndices[0]);
        //                indexBuffer.Add(indexBase + Face.mIndices[1]);
        //                indexBuffer.Add(indexBase + Face.mIndices[2]);
        //                parts[i].indexCount += 3;
        //                indexCount += 3;
        //            }
        //            */
        //        }


        //        uint vBufferSize = vertexBuffer.Count * sizeof(float);
        //        uint iBufferSize = indexBuffer.Count * sizeof(uint);

        //        // Use staging buffer to move vertex and index buffer to device local memory
        //        // Create staging buffers
        //        vksBuffer vertexStaging = new vksBuffer();
        //        vksBuffer indexStaging = new vksBuffer();

        //        // Vertex buffer
        //        Util.CheckResult(device.createBuffer(
        //            VkBufferUsageFlags.TransferSrc,
        //            VkMemoryPropertyFlags.HostVisible,
        //            vertexStaging,
        //            vBufferSize,
        //            vertexBuffer.Data.ToPointer()));

        //        // Index buffer
        //        Util.CheckResult(device.createBuffer(
        //            VkBufferUsageFlags.TransferSrc,
        //            VkMemoryPropertyFlags.HostVisible,
        //            indexStaging,
        //            iBufferSize,
        //            indexBuffer.Data.ToPointer()));

        //        // Create device local target buffers
        //        // Vertex buffer
        //        Util.CheckResult(device.createBuffer(
        //            VkBufferUsageFlags.VertexBuffer | VkBufferUsageFlags.TransferDst,
        //            VkMemoryPropertyFlags.DeviceLocal,
        //            vertices,
        //            vBufferSize));

        //        // Index buffer
        //        Util.CheckResult(device.createBuffer(
        //            VkBufferUsageFlags.IndexBuffer | VkBufferUsageFlags.TransferDst,
        //            VkMemoryPropertyFlags.DeviceLocal,
        //            indices,
        //            iBufferSize));

        //        // Copy from staging buffers
        //        VkCommandBuffer copyCmd = device.createCommandBuffer(VkCommandBufferLevel.Primary, true);

        //        VkBufferCopy copyRegion = new VkBufferCopy();

        //        copyRegion.size = vertices.size;

        //        vkCmdCopyBuffer(copyCmd, vertexStaging.buffer, vertices.buffer, 1, &copyRegion);

        //        copyRegion.size = indices.size;

        //        vkCmdCopyBuffer(copyCmd, indexStaging.buffer, indices.buffer, 1, &copyRegion);

        //        device.flushCommandBuffer(copyCmd, copyQueue);

        //        // Destroy staging resources
        //        vkDestroyBuffer(device.LogicalDevice, vertexStaging.buffer, null);

        //        vkFreeMemory(device.LogicalDevice, vertexStaging.memory, null);

        //        vkDestroyBuffer(device.LogicalDevice, indexStaging.buffer, null);

        //        vkFreeMemory(device.LogicalDevice, indexStaging.memory, null);

        //        return true;
        //    }
        //    else
        //    {
        //        Console.WriteLine("Error loading file.");
        //        return false;
        //    }
        //}


        /**
    * Loads a 3D model from a file into Vulkan buffers
    *
    * @param device Pointer to the Vulkan device used to generated the vertex and index buffers on
    * @param filename File to load (must be a model format supported by ASSIMP)
    * @param layout Vertex layout components (position, normals, tangents, etc.)
    * @param createInfo MeshCreateInfo structure for load time settings like scale, center, etc.
    * @param copyQueue Queue used for the memory staging copy commands (must support transfer)
    * @param (Optional) flags ASSIMP model loading flags
    */
        bool loadFromFile(string filename, vksVertexLayout layout, vksModelCreateInfo* createInfo, vksVulkanDevice device, VkQueue copyQueue, PostProcessSteps flags = DefaultPostProcessSteps)
        {
            this.device = device.LogicalDevice;

            // Load file
            var assimpContext = new AssimpContext();
            var pScene = assimpContext.ImportFile(filename, flags);

            parts.Clear();
            parts.Count = (uint)pScene.Meshes.Count;

            Vector3 scale = new Vector3(1.0f);
            Vector2 uvscale = new Vector2(1.0f);
            Vector3 center = new Vector3(0.0f);
            if (createInfo != null)
            {
                scale = createInfo->Scale;
                uvscale = createInfo->UVScale;
                center = createInfo->Center;
            }

            NativeList<float> vertexBuffer = new NativeList<float>();
            NativeList<uint> indexBuffer = new NativeList<uint>();

            vertexCount = 0;
            indexCount = 0;

            // Load meshes
            for (int i = 0; i < pScene.Meshes.Count; i++)
            {
                var paiMesh = pScene.Meshes[i];

                parts[i] = new ModelPart();
                parts[i].vertexBase = vertexCount;
                parts[i].indexBase = indexCount;

                vertexCount += (uint)paiMesh.VertexCount;

                var pColor = pScene.Materials[paiMesh.MaterialIndex].ColorDiffuse;

                Vector3D Zero3D = new Vector3D(0.0f, 0.0f, 0.0f);

                for (int j = 0; j < paiMesh.VertexCount; j++)
                {
                    Vector3D pPos = paiMesh.Vertices[j];
                    Vector3D pNormal = paiMesh.Normals[j];
                    Vector3D pTexCoord = paiMesh.HasTextureCoords(0) ? paiMesh.TextureCoordinateChannels[0][j] : Zero3D;
                    Vector3D pTangent = paiMesh.HasTangentBasis ? paiMesh.Tangents[j] : Zero3D;
                    Vector3D pBiTangent = paiMesh.HasTangentBasis ? paiMesh.BiTangents[j] : Zero3D;

                    foreach (var component in layout.Components)
                    {
                        switch (component)
                        {
                            case VertexComponent.VERTEX_COMPONENT_POSITION:
                                vertexBuffer.Add(pPos.X * scale.X + center.X);
                                vertexBuffer.Add(-pPos.Y * scale.Y + center.Y);
                                vertexBuffer.Add(pPos.Z * scale.Z + center.Z);
                                break;
                            case VertexComponent.VERTEX_COMPONENT_NORMAL:
                                vertexBuffer.Add(pNormal.X);
                                vertexBuffer.Add(-pNormal.Y);
                                vertexBuffer.Add(pNormal.Z);
                                break;
                            case VertexComponent.VERTEX_COMPONENT_UV:
                                vertexBuffer.Add(pTexCoord.X * uvscale.X);
                                vertexBuffer.Add(pTexCoord.Y * uvscale.Y);
                                break;
                            case VertexComponent.VERTEX_COMPONENT_COLOR:
                                vertexBuffer.Add(pColor.R);
                                vertexBuffer.Add(pColor.G);
                                vertexBuffer.Add(pColor.B);
                                break;
                            case VertexComponent.VERTEX_COMPONENT_TANGENT:
                                vertexBuffer.Add(pTangent.X);
                                vertexBuffer.Add(pTangent.Y);
                                vertexBuffer.Add(pTangent.Z);
                                break;
                            case VertexComponent.VERTEX_COMPONENT_BITANGENT:
                                vertexBuffer.Add(pBiTangent.X);
                                vertexBuffer.Add(pBiTangent.Y);
                                vertexBuffer.Add(pBiTangent.Z);
                                break;
                            // Dummy components for padding
                            case VertexComponent.VERTEX_COMPONENT_DUMMY_FLOAT:
                                vertexBuffer.Add(0.0f);
                                break;
                            case VertexComponent.VERTEX_COMPONENT_DUMMY_VEC4:
                                vertexBuffer.Add(0.0f);
                                vertexBuffer.Add(0.0f);
                                vertexBuffer.Add(0.0f);
                                vertexBuffer.Add(0.0f);
                                break;
                        };
                    }

                    dim.Max.X = (float)Math.Max(pPos.X, dim.Max.X);
                    dim.Max.Y = (float)Math.Max(pPos.Y, dim.Max.Y);
                    dim.Max.Z = (float)Math.Max(pPos.Z, dim.Max.Z);

                    dim.Min.X = (float)Math.Min(pPos.X, dim.Min.X);
                    dim.Min.Y = (float)Math.Min(pPos.Y, dim.Min.Y);
                    dim.Min.Z = (float)Math.Min(pPos.Z, dim.Min.Z);
                }

                dim.Size = dim.Max - dim.Min;

                parts[i].vertexCount = (uint)paiMesh.VertexCount;

                uint indexBase = indexBuffer.Count;
                for (uint j = 0; j < paiMesh.FaceCount; j++)
                {
                    var Face = paiMesh.Faces[(int)j];
                    if (Face.IndexCount != 3)
                        continue;
                    indexBuffer.Add(indexBase + (uint)Face.Indices[0]);
                    indexBuffer.Add(indexBase + (uint)Face.Indices[1]);
                    indexBuffer.Add(indexBase + (uint)Face.Indices[2]);
                    parts[i].indexCount += 3;
                    indexCount += 3;
                }
            }


            uint vBufferSize = (vertexBuffer.Count) * sizeof(float);
            uint iBufferSize = (indexBuffer.Count) * sizeof(uint);

            // Use staging buffer to move vertex and index buffer to device local memory
            // Create staging buffers
            vksBuffer vertexStaging = new vksBuffer();
            vksBuffer indexStaging = new vksBuffer();

            // Vertex buffer
            Util.CheckResult(device.createBuffer(
                VkBufferUsageFlags.TransferSrc,
                VkMemoryPropertyFlags.HostVisible,
                vertexStaging,
                vBufferSize,
                vertexBuffer.Data.ToPointer()));

            // Index buffer
            Util.CheckResult(device.createBuffer(
                VkBufferUsageFlags.TransferSrc,
                VkMemoryPropertyFlags.HostVisible,
                indexStaging,
                iBufferSize,
                indexBuffer.Data.ToPointer()));

            // Create device local target buffers
            // Vertex buffer
            Util.CheckResult(device.createBuffer(
                VkBufferUsageFlags.VertexBuffer | VkBufferUsageFlags.TransferDst,
                VkMemoryPropertyFlags.DeviceLocal,
                vertices,
                vBufferSize));

            // Index buffer
            Util.CheckResult(device.createBuffer(
                VkBufferUsageFlags.IndexBuffer | VkBufferUsageFlags.TransferDst,
                VkMemoryPropertyFlags.DeviceLocal,
                indices,
                iBufferSize));

            // Copy from staging buffers
            VkCommandBuffer copyCmd = device.createCommandBuffer(VkCommandBufferLevel.Primary, true);

            VkBufferCopy copyRegion = new VkBufferCopy();

            copyRegion.size = vertices.size;
            vkCmdCopyBuffer(copyCmd, vertexStaging.buffer, vertices.buffer, 1, &copyRegion);

            copyRegion.size = indices.size;
            vkCmdCopyBuffer(copyCmd, indexStaging.buffer, indices.buffer, 1, &copyRegion);

            device.flushCommandBuffer(copyCmd, copyQueue);

            // Destroy staging resources
            vkDestroyBuffer(device.LogicalDevice, vertexStaging.buffer, null);
            vkFreeMemory(device.LogicalDevice, vertexStaging.memory, null);
            vkDestroyBuffer(device.LogicalDevice, indexStaging.buffer, null);
            vkFreeMemory(device.LogicalDevice, indexStaging.memory, null);

            return true;
        }






        /**
        * Loads a 3D model from a file into Vulkan buffers
        *
        * @param device Pointer to the Vulkan device used to generated the vertex and index buffers on
        * @param filename File to load (must be a model format supported by ASSIMP)
        * @param layout Vertex layout components (position, normals, tangents, etc.)
        * @param scale Load time scene scale
        * @param copyQueue Queue used for the memory staging copy commands (must support transfer)
        * @param (Optional) flags ASSIMP model loading flags
*/
        public bool loadFromFile(string filename, vksVertexLayout layout, float scale, vksVulkanDevice device, VkQueue copyQueue, PostProcessSteps flags = DefaultPostProcessSteps)
        {
            vksModelCreateInfo modelCreateInfo = new vksModelCreateInfo(scale, 1.0f, 0.0f);
            return loadFromFile(filename, layout, &modelCreateInfo, device, copyQueue, flags);
        }
    }
}
