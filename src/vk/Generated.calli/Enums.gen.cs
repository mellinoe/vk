// This file is generated.

#if CALLI_STUBS
using System;

namespace Vulkan
{
    public enum VkImageLayout
    {
        ///<summary>Implicit layout an image is when its contents are undefined due to various reasons (e.g. right after creation)</summary>
        Undefined = 0,
        ///<summary>General layout when image can be used for any kind of access</summary>
        General = 1,
        ///<summary>Optimal layout when image is only used for color attachment read/write</summary>
        ColorAttachmentOptimal = 2,
        ///<summary>Optimal layout when image is only used for depth/stencil attachment read/write</summary>
        DepthStencilAttachmentOptimal = 3,
        ///<summary>Optimal layout when image is used for read only depth/stencil attachment and shader access</summary>
        DepthStencilReadOnlyOptimal = 4,
        ///<summary>Optimal layout when image is used for read only shader access</summary>
        ShaderReadOnlyOptimal = 5,
        ///<summary>Optimal layout when image is used only as source of transfer operations</summary>
        TransferSrcOptimal = 6,
        ///<summary>Optimal layout when image is used only as destination of transfer operations</summary>
        TransferDstOptimal = 7,
        ///<summary>Initial layout used when the data is populated by the CPU</summary>
        Preinitialized = 8,
        PresentSrc = 1000001002,
    }
    public static partial class RawConstants
    {
        ///<summary>Implicit layout an image is when its contents are undefined due to various reasons (e.g. right after creation)</summary>
        public const VkImageLayout VK_IMAGE_LAYOUT_UNDEFINED = VkImageLayout.Undefined;
        ///<summary>General layout when image can be used for any kind of access</summary>
        public const VkImageLayout VK_IMAGE_LAYOUT_GENERAL = VkImageLayout.General;
        ///<summary>Optimal layout when image is only used for color attachment read/write</summary>
        public const VkImageLayout VK_IMAGE_LAYOUT_COLOR_ATTACHMENT_OPTIMAL = VkImageLayout.ColorAttachmentOptimal;
        ///<summary>Optimal layout when image is only used for depth/stencil attachment read/write</summary>
        public const VkImageLayout VK_IMAGE_LAYOUT_DEPTH_STENCIL_ATTACHMENT_OPTIMAL = VkImageLayout.DepthStencilAttachmentOptimal;
        ///<summary>Optimal layout when image is used for read only depth/stencil attachment and shader access</summary>
        public const VkImageLayout VK_IMAGE_LAYOUT_DEPTH_STENCIL_READ_ONLY_OPTIMAL = VkImageLayout.DepthStencilReadOnlyOptimal;
        ///<summary>Optimal layout when image is used for read only shader access</summary>
        public const VkImageLayout VK_IMAGE_LAYOUT_SHADER_READ_ONLY_OPTIMAL = VkImageLayout.ShaderReadOnlyOptimal;
        ///<summary>Optimal layout when image is used only as source of transfer operations</summary>
        public const VkImageLayout VK_IMAGE_LAYOUT_TRANSFER_SRC_OPTIMAL = VkImageLayout.TransferSrcOptimal;
        ///<summary>Optimal layout when image is used only as destination of transfer operations</summary>
        public const VkImageLayout VK_IMAGE_LAYOUT_TRANSFER_DST_OPTIMAL = VkImageLayout.TransferDstOptimal;
        ///<summary>Initial layout used when the data is populated by the CPU</summary>
        public const VkImageLayout VK_IMAGE_LAYOUT_PREINITIALIZED = VkImageLayout.Preinitialized;
        public const VkImageLayout VK_IMAGE_LAYOUT_PRESENT_SRC_KHR = VkImageLayout.PresentSrc;
    }

    public enum VkAttachmentLoadOp
    {
        Load = 0,
        Clear = 1,
        DontCare = 2,
    }
    public static partial class RawConstants
    {
        public const VkAttachmentLoadOp VK_ATTACHMENT_LOAD_OP_LOAD = VkAttachmentLoadOp.Load;
        public const VkAttachmentLoadOp VK_ATTACHMENT_LOAD_OP_CLEAR = VkAttachmentLoadOp.Clear;
        public const VkAttachmentLoadOp VK_ATTACHMENT_LOAD_OP_DONT_CARE = VkAttachmentLoadOp.DontCare;
    }

    public enum VkAttachmentStoreOp
    {
        Store = 0,
        DontCare = 1,
    }
    public static partial class RawConstants
    {
        public const VkAttachmentStoreOp VK_ATTACHMENT_STORE_OP_STORE = VkAttachmentStoreOp.Store;
        public const VkAttachmentStoreOp VK_ATTACHMENT_STORE_OP_DONT_CARE = VkAttachmentStoreOp.DontCare;
    }

    public enum VkImageType
    {
        _1d = 0,
        _2d = 1,
        _3d = 2,
    }
    public static partial class RawConstants
    {
        public const VkImageType VK_IMAGE_TYPE_1D = VkImageType._1d;
        public const VkImageType VK_IMAGE_TYPE_2D = VkImageType._2d;
        public const VkImageType VK_IMAGE_TYPE_3D = VkImageType._3d;
    }

    public enum VkImageTiling
    {
        Optimal = 0,
        Linear = 1,
    }
    public static partial class RawConstants
    {
        public const VkImageTiling VK_IMAGE_TILING_OPTIMAL = VkImageTiling.Optimal;
        public const VkImageTiling VK_IMAGE_TILING_LINEAR = VkImageTiling.Linear;
    }

    public enum VkImageViewType
    {
        _1d = 0,
        _2d = 1,
        _3d = 2,
        Cube = 3,
        _1dArray = 4,
        _2dArray = 5,
        CubeArray = 6,
    }
    public static partial class RawConstants
    {
        public const VkImageViewType VK_IMAGE_VIEW_TYPE_1D = VkImageViewType._1d;
        public const VkImageViewType VK_IMAGE_VIEW_TYPE_2D = VkImageViewType._2d;
        public const VkImageViewType VK_IMAGE_VIEW_TYPE_3D = VkImageViewType._3d;
        public const VkImageViewType VK_IMAGE_VIEW_TYPE_CUBE = VkImageViewType.Cube;
        public const VkImageViewType VK_IMAGE_VIEW_TYPE_1D_ARRAY = VkImageViewType._1dArray;
        public const VkImageViewType VK_IMAGE_VIEW_TYPE_2D_ARRAY = VkImageViewType._2dArray;
        public const VkImageViewType VK_IMAGE_VIEW_TYPE_CUBE_ARRAY = VkImageViewType.CubeArray;
    }

    public enum VkCommandBufferLevel
    {
        Primary = 0,
        Secondary = 1,
    }
    public static partial class RawConstants
    {
        public const VkCommandBufferLevel VK_COMMAND_BUFFER_LEVEL_PRIMARY = VkCommandBufferLevel.Primary;
        public const VkCommandBufferLevel VK_COMMAND_BUFFER_LEVEL_SECONDARY = VkCommandBufferLevel.Secondary;
    }

    public enum VkComponentSwizzle
    {
        Identity = 0,
        Zero = 1,
        One = 2,
        R = 3,
        G = 4,
        B = 5,
        A = 6,
    }
    public static partial class RawConstants
    {
        public const VkComponentSwizzle VK_COMPONENT_SWIZZLE_IDENTITY = VkComponentSwizzle.Identity;
        public const VkComponentSwizzle VK_COMPONENT_SWIZZLE_ZERO = VkComponentSwizzle.Zero;
        public const VkComponentSwizzle VK_COMPONENT_SWIZZLE_ONE = VkComponentSwizzle.One;
        public const VkComponentSwizzle VK_COMPONENT_SWIZZLE_R = VkComponentSwizzle.R;
        public const VkComponentSwizzle VK_COMPONENT_SWIZZLE_G = VkComponentSwizzle.G;
        public const VkComponentSwizzle VK_COMPONENT_SWIZZLE_B = VkComponentSwizzle.B;
        public const VkComponentSwizzle VK_COMPONENT_SWIZZLE_A = VkComponentSwizzle.A;
    }

    public enum VkDescriptorType
    {
        Sampler = 0,
        CombinedImageSampler = 1,
        SampledImage = 2,
        StorageImage = 3,
        UniformTexelBuffer = 4,
        StorageTexelBuffer = 5,
        UniformBuffer = 6,
        StorageBuffer = 7,
        UniformBufferDynamic = 8,
        StorageBufferDynamic = 9,
        InputAttachment = 10,
    }
    public static partial class RawConstants
    {
        public const VkDescriptorType VK_DESCRIPTOR_TYPE_SAMPLER = VkDescriptorType.Sampler;
        public const VkDescriptorType VK_DESCRIPTOR_TYPE_COMBINED_IMAGE_SAMPLER = VkDescriptorType.CombinedImageSampler;
        public const VkDescriptorType VK_DESCRIPTOR_TYPE_SAMPLED_IMAGE = VkDescriptorType.SampledImage;
        public const VkDescriptorType VK_DESCRIPTOR_TYPE_STORAGE_IMAGE = VkDescriptorType.StorageImage;
        public const VkDescriptorType VK_DESCRIPTOR_TYPE_UNIFORM_TEXEL_BUFFER = VkDescriptorType.UniformTexelBuffer;
        public const VkDescriptorType VK_DESCRIPTOR_TYPE_STORAGE_TEXEL_BUFFER = VkDescriptorType.StorageTexelBuffer;
        public const VkDescriptorType VK_DESCRIPTOR_TYPE_UNIFORM_BUFFER = VkDescriptorType.UniformBuffer;
        public const VkDescriptorType VK_DESCRIPTOR_TYPE_STORAGE_BUFFER = VkDescriptorType.StorageBuffer;
        public const VkDescriptorType VK_DESCRIPTOR_TYPE_UNIFORM_BUFFER_DYNAMIC = VkDescriptorType.UniformBufferDynamic;
        public const VkDescriptorType VK_DESCRIPTOR_TYPE_STORAGE_BUFFER_DYNAMIC = VkDescriptorType.StorageBufferDynamic;
        public const VkDescriptorType VK_DESCRIPTOR_TYPE_INPUT_ATTACHMENT = VkDescriptorType.InputAttachment;
    }

    public enum VkQueryType
    {
        Occlusion = 0,
        ///<summary>Optional</summary>
        PipelineStatistics = 1,
        Timestamp = 2,
    }
    public static partial class RawConstants
    {
        public const VkQueryType VK_QUERY_TYPE_OCCLUSION = VkQueryType.Occlusion;
        ///<summary>Optional</summary>
        public const VkQueryType VK_QUERY_TYPE_PIPELINE_STATISTICS = VkQueryType.PipelineStatistics;
        public const VkQueryType VK_QUERY_TYPE_TIMESTAMP = VkQueryType.Timestamp;
    }

    public enum VkBorderColor
    {
        FloatTransparentBlack = 0,
        IntTransparentBlack = 1,
        FloatOpaqueBlack = 2,
        IntOpaqueBlack = 3,
        FloatOpaqueWhite = 4,
        IntOpaqueWhite = 5,
    }
    public static partial class RawConstants
    {
        public const VkBorderColor VK_BORDER_COLOR_FLOAT_TRANSPARENT_BLACK = VkBorderColor.FloatTransparentBlack;
        public const VkBorderColor VK_BORDER_COLOR_INT_TRANSPARENT_BLACK = VkBorderColor.IntTransparentBlack;
        public const VkBorderColor VK_BORDER_COLOR_FLOAT_OPAQUE_BLACK = VkBorderColor.FloatOpaqueBlack;
        public const VkBorderColor VK_BORDER_COLOR_INT_OPAQUE_BLACK = VkBorderColor.IntOpaqueBlack;
        public const VkBorderColor VK_BORDER_COLOR_FLOAT_OPAQUE_WHITE = VkBorderColor.FloatOpaqueWhite;
        public const VkBorderColor VK_BORDER_COLOR_INT_OPAQUE_WHITE = VkBorderColor.IntOpaqueWhite;
    }

    public enum VkPipelineBindPoint
    {
        Graphics = 0,
        Compute = 1,
    }
    public static partial class RawConstants
    {
        public const VkPipelineBindPoint VK_PIPELINE_BIND_POINT_GRAPHICS = VkPipelineBindPoint.Graphics;
        public const VkPipelineBindPoint VK_PIPELINE_BIND_POINT_COMPUTE = VkPipelineBindPoint.Compute;
    }

    public enum VkPipelineCacheHeaderVersion
    {
        One = 1,
    }
    public static partial class RawConstants
    {
        public const VkPipelineCacheHeaderVersion VK_PIPELINE_CACHE_HEADER_VERSION_ONE = VkPipelineCacheHeaderVersion.One;
    }

    public enum VkPrimitiveTopology
    {
        PointList = 0,
        LineList = 1,
        LineStrip = 2,
        TriangleList = 3,
        TriangleStrip = 4,
        TriangleFan = 5,
        LineListWithAdjacency = 6,
        LineStripWithAdjacency = 7,
        TriangleListWithAdjacency = 8,
        TriangleStripWithAdjacency = 9,
        PatchList = 10,
    }
    public static partial class RawConstants
    {
        public const VkPrimitiveTopology VK_PRIMITIVE_TOPOLOGY_POINT_LIST = VkPrimitiveTopology.PointList;
        public const VkPrimitiveTopology VK_PRIMITIVE_TOPOLOGY_LINE_LIST = VkPrimitiveTopology.LineList;
        public const VkPrimitiveTopology VK_PRIMITIVE_TOPOLOGY_LINE_STRIP = VkPrimitiveTopology.LineStrip;
        public const VkPrimitiveTopology VK_PRIMITIVE_TOPOLOGY_TRIANGLE_LIST = VkPrimitiveTopology.TriangleList;
        public const VkPrimitiveTopology VK_PRIMITIVE_TOPOLOGY_TRIANGLE_STRIP = VkPrimitiveTopology.TriangleStrip;
        public const VkPrimitiveTopology VK_PRIMITIVE_TOPOLOGY_TRIANGLE_FAN = VkPrimitiveTopology.TriangleFan;
        public const VkPrimitiveTopology VK_PRIMITIVE_TOPOLOGY_LINE_LIST_WITH_ADJACENCY = VkPrimitiveTopology.LineListWithAdjacency;
        public const VkPrimitiveTopology VK_PRIMITIVE_TOPOLOGY_LINE_STRIP_WITH_ADJACENCY = VkPrimitiveTopology.LineStripWithAdjacency;
        public const VkPrimitiveTopology VK_PRIMITIVE_TOPOLOGY_TRIANGLE_LIST_WITH_ADJACENCY = VkPrimitiveTopology.TriangleListWithAdjacency;
        public const VkPrimitiveTopology VK_PRIMITIVE_TOPOLOGY_TRIANGLE_STRIP_WITH_ADJACENCY = VkPrimitiveTopology.TriangleStripWithAdjacency;
        public const VkPrimitiveTopology VK_PRIMITIVE_TOPOLOGY_PATCH_LIST = VkPrimitiveTopology.PatchList;
    }

    public enum VkSharingMode
    {
        Exclusive = 0,
        Concurrent = 1,
    }
    public static partial class RawConstants
    {
        public const VkSharingMode VK_SHARING_MODE_EXCLUSIVE = VkSharingMode.Exclusive;
        public const VkSharingMode VK_SHARING_MODE_CONCURRENT = VkSharingMode.Concurrent;
    }

    public enum VkIndexType
    {
        Uint16 = 0,
        Uint32 = 1,
    }
    public static partial class RawConstants
    {
        public const VkIndexType VK_INDEX_TYPE_UINT16 = VkIndexType.Uint16;
        public const VkIndexType VK_INDEX_TYPE_UINT32 = VkIndexType.Uint32;
    }

    public enum VkFilter
    {
        Nearest = 0,
        Linear = 1,
        CubicImg = 1000015000,
    }
    public static partial class RawConstants
    {
        public const VkFilter VK_FILTER_NEAREST = VkFilter.Nearest;
        public const VkFilter VK_FILTER_LINEAR = VkFilter.Linear;
        public const VkFilter VK_FILTER_CUBIC_IMG = VkFilter.CubicImg;
    }

    public enum VkSamplerMipmapMode
    {
        ///<summary>Choose nearest mip level</summary>
        Nearest = 0,
        ///<summary>Linear filter between mip levels</summary>
        Linear = 1,
    }
    public static partial class RawConstants
    {
        ///<summary>Choose nearest mip level</summary>
        public const VkSamplerMipmapMode VK_SAMPLER_MIPMAP_MODE_NEAREST = VkSamplerMipmapMode.Nearest;
        ///<summary>Linear filter between mip levels</summary>
        public const VkSamplerMipmapMode VK_SAMPLER_MIPMAP_MODE_LINEAR = VkSamplerMipmapMode.Linear;
    }

    public enum VkSamplerAddressMode
    {
        Repeat = 0,
        MirroredRepeat = 1,
        ClampToEdge = 2,
        ClampToBorder = 3,
        MirrorClampToEdge = 4,
    }
    public static partial class RawConstants
    {
        public const VkSamplerAddressMode VK_SAMPLER_ADDRESS_MODE_REPEAT = VkSamplerAddressMode.Repeat;
        public const VkSamplerAddressMode VK_SAMPLER_ADDRESS_MODE_MIRRORED_REPEAT = VkSamplerAddressMode.MirroredRepeat;
        public const VkSamplerAddressMode VK_SAMPLER_ADDRESS_MODE_CLAMP_TO_EDGE = VkSamplerAddressMode.ClampToEdge;
        public const VkSamplerAddressMode VK_SAMPLER_ADDRESS_MODE_CLAMP_TO_BORDER = VkSamplerAddressMode.ClampToBorder;
        public const VkSamplerAddressMode VK_SAMPLER_ADDRESS_MODE_MIRROR_CLAMP_TO_EDGE = VkSamplerAddressMode.MirrorClampToEdge;
    }

    public enum VkCompareOp
    {
        Never = 0,
        Less = 1,
        Equal = 2,
        LessOrEqual = 3,
        Greater = 4,
        NotEqual = 5,
        GreaterOrEqual = 6,
        Always = 7,
    }
    public static partial class RawConstants
    {
        public const VkCompareOp VK_COMPARE_OP_NEVER = VkCompareOp.Never;
        public const VkCompareOp VK_COMPARE_OP_LESS = VkCompareOp.Less;
        public const VkCompareOp VK_COMPARE_OP_EQUAL = VkCompareOp.Equal;
        public const VkCompareOp VK_COMPARE_OP_LESS_OR_EQUAL = VkCompareOp.LessOrEqual;
        public const VkCompareOp VK_COMPARE_OP_GREATER = VkCompareOp.Greater;
        public const VkCompareOp VK_COMPARE_OP_NOT_EQUAL = VkCompareOp.NotEqual;
        public const VkCompareOp VK_COMPARE_OP_GREATER_OR_EQUAL = VkCompareOp.GreaterOrEqual;
        public const VkCompareOp VK_COMPARE_OP_ALWAYS = VkCompareOp.Always;
    }

    public enum VkPolygonMode
    {
        Fill = 0,
        Line = 1,
        Point = 2,
    }
    public static partial class RawConstants
    {
        public const VkPolygonMode VK_POLYGON_MODE_FILL = VkPolygonMode.Fill;
        public const VkPolygonMode VK_POLYGON_MODE_LINE = VkPolygonMode.Line;
        public const VkPolygonMode VK_POLYGON_MODE_POINT = VkPolygonMode.Point;
    }

    [Flags]
    public enum VkCullModeFlags
    {
        None = 0,
        Front = 1,
        Back = 2,
        FrontAndBack = 3,
    }
    public static partial class RawConstants
    {
        public const VkCullModeFlags VK_CULL_MODE_NONE = VkCullModeFlags.None;
        public const VkCullModeFlags VK_CULL_MODE_FRONT_BIT = VkCullModeFlags.Front;
        public const VkCullModeFlags VK_CULL_MODE_BACK_BIT = VkCullModeFlags.Back;
        public const VkCullModeFlags VK_CULL_MODE_FRONT_AND_BACK = VkCullModeFlags.FrontAndBack;
    }

    public enum VkFrontFace
    {
        CounterClockwise = 0,
        Clockwise = 1,
    }
    public static partial class RawConstants
    {
        public const VkFrontFace VK_FRONT_FACE_COUNTER_CLOCKWISE = VkFrontFace.CounterClockwise;
        public const VkFrontFace VK_FRONT_FACE_CLOCKWISE = VkFrontFace.Clockwise;
    }

    public enum VkBlendFactor
    {
        Zero = 0,
        One = 1,
        SrcColor = 2,
        OneMinusSrcColor = 3,
        DstColor = 4,
        OneMinusDstColor = 5,
        SrcAlpha = 6,
        OneMinusSrcAlpha = 7,
        DstAlpha = 8,
        OneMinusDstAlpha = 9,
        ConstantColor = 10,
        OneMinusConstantColor = 11,
        ConstantAlpha = 12,
        OneMinusConstantAlpha = 13,
        SrcAlphaSaturate = 14,
        Src1Color = 15,
        OneMinusSrc1Color = 16,
        Src1Alpha = 17,
        OneMinusSrc1Alpha = 18,
    }
    public static partial class RawConstants
    {
        public const VkBlendFactor VK_BLEND_FACTOR_ZERO = VkBlendFactor.Zero;
        public const VkBlendFactor VK_BLEND_FACTOR_ONE = VkBlendFactor.One;
        public const VkBlendFactor VK_BLEND_FACTOR_SRC_COLOR = VkBlendFactor.SrcColor;
        public const VkBlendFactor VK_BLEND_FACTOR_ONE_MINUS_SRC_COLOR = VkBlendFactor.OneMinusSrcColor;
        public const VkBlendFactor VK_BLEND_FACTOR_DST_COLOR = VkBlendFactor.DstColor;
        public const VkBlendFactor VK_BLEND_FACTOR_ONE_MINUS_DST_COLOR = VkBlendFactor.OneMinusDstColor;
        public const VkBlendFactor VK_BLEND_FACTOR_SRC_ALPHA = VkBlendFactor.SrcAlpha;
        public const VkBlendFactor VK_BLEND_FACTOR_ONE_MINUS_SRC_ALPHA = VkBlendFactor.OneMinusSrcAlpha;
        public const VkBlendFactor VK_BLEND_FACTOR_DST_ALPHA = VkBlendFactor.DstAlpha;
        public const VkBlendFactor VK_BLEND_FACTOR_ONE_MINUS_DST_ALPHA = VkBlendFactor.OneMinusDstAlpha;
        public const VkBlendFactor VK_BLEND_FACTOR_CONSTANT_COLOR = VkBlendFactor.ConstantColor;
        public const VkBlendFactor VK_BLEND_FACTOR_ONE_MINUS_CONSTANT_COLOR = VkBlendFactor.OneMinusConstantColor;
        public const VkBlendFactor VK_BLEND_FACTOR_CONSTANT_ALPHA = VkBlendFactor.ConstantAlpha;
        public const VkBlendFactor VK_BLEND_FACTOR_ONE_MINUS_CONSTANT_ALPHA = VkBlendFactor.OneMinusConstantAlpha;
        public const VkBlendFactor VK_BLEND_FACTOR_SRC_ALPHA_SATURATE = VkBlendFactor.SrcAlphaSaturate;
        public const VkBlendFactor VK_BLEND_FACTOR_SRC1_COLOR = VkBlendFactor.Src1Color;
        public const VkBlendFactor VK_BLEND_FACTOR_ONE_MINUS_SRC1_COLOR = VkBlendFactor.OneMinusSrc1Color;
        public const VkBlendFactor VK_BLEND_FACTOR_SRC1_ALPHA = VkBlendFactor.Src1Alpha;
        public const VkBlendFactor VK_BLEND_FACTOR_ONE_MINUS_SRC1_ALPHA = VkBlendFactor.OneMinusSrc1Alpha;
    }

    public enum VkBlendOp
    {
        Add = 0,
        Subtract = 1,
        ReverseSubtract = 2,
        Min = 3,
        Max = 4,
    }
    public static partial class RawConstants
    {
        public const VkBlendOp VK_BLEND_OP_ADD = VkBlendOp.Add;
        public const VkBlendOp VK_BLEND_OP_SUBTRACT = VkBlendOp.Subtract;
        public const VkBlendOp VK_BLEND_OP_REVERSE_SUBTRACT = VkBlendOp.ReverseSubtract;
        public const VkBlendOp VK_BLEND_OP_MIN = VkBlendOp.Min;
        public const VkBlendOp VK_BLEND_OP_MAX = VkBlendOp.Max;
    }

    public enum VkStencilOp
    {
        Keep = 0,
        Zero = 1,
        Replace = 2,
        IncrementAndClamp = 3,
        DecrementAndClamp = 4,
        Invert = 5,
        IncrementAndWrap = 6,
        DecrementAndWrap = 7,
    }
    public static partial class RawConstants
    {
        public const VkStencilOp VK_STENCIL_OP_KEEP = VkStencilOp.Keep;
        public const VkStencilOp VK_STENCIL_OP_ZERO = VkStencilOp.Zero;
        public const VkStencilOp VK_STENCIL_OP_REPLACE = VkStencilOp.Replace;
        public const VkStencilOp VK_STENCIL_OP_INCREMENT_AND_CLAMP = VkStencilOp.IncrementAndClamp;
        public const VkStencilOp VK_STENCIL_OP_DECREMENT_AND_CLAMP = VkStencilOp.DecrementAndClamp;
        public const VkStencilOp VK_STENCIL_OP_INVERT = VkStencilOp.Invert;
        public const VkStencilOp VK_STENCIL_OP_INCREMENT_AND_WRAP = VkStencilOp.IncrementAndWrap;
        public const VkStencilOp VK_STENCIL_OP_DECREMENT_AND_WRAP = VkStencilOp.DecrementAndWrap;
    }

    public enum VkLogicOp
    {
        Clear = 0,
        And = 1,
        AndReverse = 2,
        Copy = 3,
        AndInverted = 4,
        NoOp = 5,
        Xor = 6,
        Or = 7,
        Nor = 8,
        Equivalent = 9,
        Invert = 10,
        OrReverse = 11,
        CopyInverted = 12,
        OrInverted = 13,
        Nand = 14,
        Set = 15,
    }
    public static partial class RawConstants
    {
        public const VkLogicOp VK_LOGIC_OP_CLEAR = VkLogicOp.Clear;
        public const VkLogicOp VK_LOGIC_OP_AND = VkLogicOp.And;
        public const VkLogicOp VK_LOGIC_OP_AND_REVERSE = VkLogicOp.AndReverse;
        public const VkLogicOp VK_LOGIC_OP_COPY = VkLogicOp.Copy;
        public const VkLogicOp VK_LOGIC_OP_AND_INVERTED = VkLogicOp.AndInverted;
        public const VkLogicOp VK_LOGIC_OP_NO_OP = VkLogicOp.NoOp;
        public const VkLogicOp VK_LOGIC_OP_XOR = VkLogicOp.Xor;
        public const VkLogicOp VK_LOGIC_OP_OR = VkLogicOp.Or;
        public const VkLogicOp VK_LOGIC_OP_NOR = VkLogicOp.Nor;
        public const VkLogicOp VK_LOGIC_OP_EQUIVALENT = VkLogicOp.Equivalent;
        public const VkLogicOp VK_LOGIC_OP_INVERT = VkLogicOp.Invert;
        public const VkLogicOp VK_LOGIC_OP_OR_REVERSE = VkLogicOp.OrReverse;
        public const VkLogicOp VK_LOGIC_OP_COPY_INVERTED = VkLogicOp.CopyInverted;
        public const VkLogicOp VK_LOGIC_OP_OR_INVERTED = VkLogicOp.OrInverted;
        public const VkLogicOp VK_LOGIC_OP_NAND = VkLogicOp.Nand;
        public const VkLogicOp VK_LOGIC_OP_SET = VkLogicOp.Set;
    }

    public enum VkInternalAllocationType
    {
        Executable = 0,
    }
    public static partial class RawConstants
    {
        public const VkInternalAllocationType VK_INTERNAL_ALLOCATION_TYPE_EXECUTABLE = VkInternalAllocationType.Executable;
    }

    public enum VkSystemAllocationScope
    {
        Command = 0,
        Object = 1,
        Cache = 2,
        Device = 3,
        Instance = 4,
    }
    public static partial class RawConstants
    {
        public const VkSystemAllocationScope VK_SYSTEM_ALLOCATION_SCOPE_COMMAND = VkSystemAllocationScope.Command;
        public const VkSystemAllocationScope VK_SYSTEM_ALLOCATION_SCOPE_OBJECT = VkSystemAllocationScope.Object;
        public const VkSystemAllocationScope VK_SYSTEM_ALLOCATION_SCOPE_CACHE = VkSystemAllocationScope.Cache;
        public const VkSystemAllocationScope VK_SYSTEM_ALLOCATION_SCOPE_DEVICE = VkSystemAllocationScope.Device;
        public const VkSystemAllocationScope VK_SYSTEM_ALLOCATION_SCOPE_INSTANCE = VkSystemAllocationScope.Instance;
    }

    public enum VkPhysicalDeviceType
    {
        Other = 0,
        IntegratedGpu = 1,
        DiscreteGpu = 2,
        VirtualGpu = 3,
        Cpu = 4,
    }
    public static partial class RawConstants
    {
        public const VkPhysicalDeviceType VK_PHYSICAL_DEVICE_TYPE_OTHER = VkPhysicalDeviceType.Other;
        public const VkPhysicalDeviceType VK_PHYSICAL_DEVICE_TYPE_INTEGRATED_GPU = VkPhysicalDeviceType.IntegratedGpu;
        public const VkPhysicalDeviceType VK_PHYSICAL_DEVICE_TYPE_DISCRETE_GPU = VkPhysicalDeviceType.DiscreteGpu;
        public const VkPhysicalDeviceType VK_PHYSICAL_DEVICE_TYPE_VIRTUAL_GPU = VkPhysicalDeviceType.VirtualGpu;
        public const VkPhysicalDeviceType VK_PHYSICAL_DEVICE_TYPE_CPU = VkPhysicalDeviceType.Cpu;
    }

    public enum VkVertexInputRate
    {
        Vertex = 0,
        Instance = 1,
    }
    public static partial class RawConstants
    {
        public const VkVertexInputRate VK_VERTEX_INPUT_RATE_VERTEX = VkVertexInputRate.Vertex;
        public const VkVertexInputRate VK_VERTEX_INPUT_RATE_INSTANCE = VkVertexInputRate.Instance;
    }

    public enum VkFormat
    {
        Undefined = 0,
        R4g4UnormPack8 = 1,
        R4g4b4a4UnormPack16 = 2,
        B4g4r4a4UnormPack16 = 3,
        R5g6b5UnormPack16 = 4,
        B5g6r5UnormPack16 = 5,
        R5g5b5a1UnormPack16 = 6,
        B5g5r5a1UnormPack16 = 7,
        A1r5g5b5UnormPack16 = 8,
        R8Unorm = 9,
        R8Snorm = 10,
        R8Uscaled = 11,
        R8Sscaled = 12,
        R8Uint = 13,
        R8Sint = 14,
        R8Srgb = 15,
        R8g8Unorm = 16,
        R8g8Snorm = 17,
        R8g8Uscaled = 18,
        R8g8Sscaled = 19,
        R8g8Uint = 20,
        R8g8Sint = 21,
        R8g8Srgb = 22,
        R8g8b8Unorm = 23,
        R8g8b8Snorm = 24,
        R8g8b8Uscaled = 25,
        R8g8b8Sscaled = 26,
        R8g8b8Uint = 27,
        R8g8b8Sint = 28,
        R8g8b8Srgb = 29,
        B8g8r8Unorm = 30,
        B8g8r8Snorm = 31,
        B8g8r8Uscaled = 32,
        B8g8r8Sscaled = 33,
        B8g8r8Uint = 34,
        B8g8r8Sint = 35,
        B8g8r8Srgb = 36,
        R8g8b8a8Unorm = 37,
        R8g8b8a8Snorm = 38,
        R8g8b8a8Uscaled = 39,
        R8g8b8a8Sscaled = 40,
        R8g8b8a8Uint = 41,
        R8g8b8a8Sint = 42,
        R8g8b8a8Srgb = 43,
        B8g8r8a8Unorm = 44,
        B8g8r8a8Snorm = 45,
        B8g8r8a8Uscaled = 46,
        B8g8r8a8Sscaled = 47,
        B8g8r8a8Uint = 48,
        B8g8r8a8Sint = 49,
        B8g8r8a8Srgb = 50,
        A8b8g8r8UnormPack32 = 51,
        A8b8g8r8SnormPack32 = 52,
        A8b8g8r8UscaledPack32 = 53,
        A8b8g8r8SscaledPack32 = 54,
        A8b8g8r8UintPack32 = 55,
        A8b8g8r8SintPack32 = 56,
        A8b8g8r8SrgbPack32 = 57,
        A2r10g10b10UnormPack32 = 58,
        A2r10g10b10SnormPack32 = 59,
        A2r10g10b10UscaledPack32 = 60,
        A2r10g10b10SscaledPack32 = 61,
        A2r10g10b10UintPack32 = 62,
        A2r10g10b10SintPack32 = 63,
        A2b10g10r10UnormPack32 = 64,
        A2b10g10r10SnormPack32 = 65,
        A2b10g10r10UscaledPack32 = 66,
        A2b10g10r10SscaledPack32 = 67,
        A2b10g10r10UintPack32 = 68,
        A2b10g10r10SintPack32 = 69,
        R16Unorm = 70,
        R16Snorm = 71,
        R16Uscaled = 72,
        R16Sscaled = 73,
        R16Uint = 74,
        R16Sint = 75,
        R16Sfloat = 76,
        R16g16Unorm = 77,
        R16g16Snorm = 78,
        R16g16Uscaled = 79,
        R16g16Sscaled = 80,
        R16g16Uint = 81,
        R16g16Sint = 82,
        R16g16Sfloat = 83,
        R16g16b16Unorm = 84,
        R16g16b16Snorm = 85,
        R16g16b16Uscaled = 86,
        R16g16b16Sscaled = 87,
        R16g16b16Uint = 88,
        R16g16b16Sint = 89,
        R16g16b16Sfloat = 90,
        R16g16b16a16Unorm = 91,
        R16g16b16a16Snorm = 92,
        R16g16b16a16Uscaled = 93,
        R16g16b16a16Sscaled = 94,
        R16g16b16a16Uint = 95,
        R16g16b16a16Sint = 96,
        R16g16b16a16Sfloat = 97,
        R32Uint = 98,
        R32Sint = 99,
        R32Sfloat = 100,
        R32g32Uint = 101,
        R32g32Sint = 102,
        R32g32Sfloat = 103,
        R32g32b32Uint = 104,
        R32g32b32Sint = 105,
        R32g32b32Sfloat = 106,
        R32g32b32a32Uint = 107,
        R32g32b32a32Sint = 108,
        R32g32b32a32Sfloat = 109,
        R64Uint = 110,
        R64Sint = 111,
        R64Sfloat = 112,
        R64g64Uint = 113,
        R64g64Sint = 114,
        R64g64Sfloat = 115,
        R64g64b64Uint = 116,
        R64g64b64Sint = 117,
        R64g64b64Sfloat = 118,
        R64g64b64a64Uint = 119,
        R64g64b64a64Sint = 120,
        R64g64b64a64Sfloat = 121,
        B10g11r11UfloatPack32 = 122,
        E5b9g9r9UfloatPack32 = 123,
        D16Unorm = 124,
        X8D24UnormPack32 = 125,
        D32Sfloat = 126,
        S8Uint = 127,
        D16UnormS8Uint = 128,
        D24UnormS8Uint = 129,
        D32SfloatS8Uint = 130,
        Bc1RgbUnormBlock = 131,
        Bc1RgbSrgbBlock = 132,
        Bc1RgbaUnormBlock = 133,
        Bc1RgbaSrgbBlock = 134,
        Bc2UnormBlock = 135,
        Bc2SrgbBlock = 136,
        Bc3UnormBlock = 137,
        Bc3SrgbBlock = 138,
        Bc4UnormBlock = 139,
        Bc4SnormBlock = 140,
        Bc5UnormBlock = 141,
        Bc5SnormBlock = 142,
        Bc6hUfloatBlock = 143,
        Bc6hSfloatBlock = 144,
        Bc7UnormBlock = 145,
        Bc7SrgbBlock = 146,
        Etc2R8g8b8UnormBlock = 147,
        Etc2R8g8b8SrgbBlock = 148,
        Etc2R8g8b8a1UnormBlock = 149,
        Etc2R8g8b8a1SrgbBlock = 150,
        Etc2R8g8b8a8UnormBlock = 151,
        Etc2R8g8b8a8SrgbBlock = 152,
        EacR11UnormBlock = 153,
        EacR11SnormBlock = 154,
        EacR11g11UnormBlock = 155,
        EacR11g11SnormBlock = 156,
        Astc4x4UnormBlock = 157,
        Astc4x4SrgbBlock = 158,
        Astc5x4UnormBlock = 159,
        Astc5x4SrgbBlock = 160,
        Astc5x5UnormBlock = 161,
        Astc5x5SrgbBlock = 162,
        Astc6x5UnormBlock = 163,
        Astc6x5SrgbBlock = 164,
        Astc6x6UnormBlock = 165,
        Astc6x6SrgbBlock = 166,
        Astc8x5UnormBlock = 167,
        Astc8x5SrgbBlock = 168,
        Astc8x6UnormBlock = 169,
        Astc8x6SrgbBlock = 170,
        Astc8x8UnormBlock = 171,
        Astc8x8SrgbBlock = 172,
        Astc10x5UnormBlock = 173,
        Astc10x5SrgbBlock = 174,
        Astc10x6UnormBlock = 175,
        Astc10x6SrgbBlock = 176,
        Astc10x8UnormBlock = 177,
        Astc10x8SrgbBlock = 178,
        Astc10x10UnormBlock = 179,
        Astc10x10SrgbBlock = 180,
        Astc12x10UnormBlock = 181,
        Astc12x10SrgbBlock = 182,
        Astc12x12UnormBlock = 183,
        Astc12x12SrgbBlock = 184,
        Pvrtc12bppUnormBlockImg = 1000054000,
        Pvrtc14bppUnormBlockImg = 1000054001,
        Pvrtc22bppUnormBlockImg = 1000054002,
        Pvrtc24bppUnormBlockImg = 1000054003,
        Pvrtc12bppSrgbBlockImg = 1000054004,
        Pvrtc14bppSrgbBlockImg = 1000054005,
        Pvrtc22bppSrgbBlockImg = 1000054006,
        Pvrtc24bppSrgbBlockImg = 1000054007,
    }
    public static partial class RawConstants
    {
        public const VkFormat VK_FORMAT_UNDEFINED = VkFormat.Undefined;
        public const VkFormat VK_FORMAT_R4G4_UNORM_PACK8 = VkFormat.R4g4UnormPack8;
        public const VkFormat VK_FORMAT_R4G4B4A4_UNORM_PACK16 = VkFormat.R4g4b4a4UnormPack16;
        public const VkFormat VK_FORMAT_B4G4R4A4_UNORM_PACK16 = VkFormat.B4g4r4a4UnormPack16;
        public const VkFormat VK_FORMAT_R5G6B5_UNORM_PACK16 = VkFormat.R5g6b5UnormPack16;
        public const VkFormat VK_FORMAT_B5G6R5_UNORM_PACK16 = VkFormat.B5g6r5UnormPack16;
        public const VkFormat VK_FORMAT_R5G5B5A1_UNORM_PACK16 = VkFormat.R5g5b5a1UnormPack16;
        public const VkFormat VK_FORMAT_B5G5R5A1_UNORM_PACK16 = VkFormat.B5g5r5a1UnormPack16;
        public const VkFormat VK_FORMAT_A1R5G5B5_UNORM_PACK16 = VkFormat.A1r5g5b5UnormPack16;
        public const VkFormat VK_FORMAT_R8_UNORM = VkFormat.R8Unorm;
        public const VkFormat VK_FORMAT_R8_SNORM = VkFormat.R8Snorm;
        public const VkFormat VK_FORMAT_R8_USCALED = VkFormat.R8Uscaled;
        public const VkFormat VK_FORMAT_R8_SSCALED = VkFormat.R8Sscaled;
        public const VkFormat VK_FORMAT_R8_UINT = VkFormat.R8Uint;
        public const VkFormat VK_FORMAT_R8_SINT = VkFormat.R8Sint;
        public const VkFormat VK_FORMAT_R8_SRGB = VkFormat.R8Srgb;
        public const VkFormat VK_FORMAT_R8G8_UNORM = VkFormat.R8g8Unorm;
        public const VkFormat VK_FORMAT_R8G8_SNORM = VkFormat.R8g8Snorm;
        public const VkFormat VK_FORMAT_R8G8_USCALED = VkFormat.R8g8Uscaled;
        public const VkFormat VK_FORMAT_R8G8_SSCALED = VkFormat.R8g8Sscaled;
        public const VkFormat VK_FORMAT_R8G8_UINT = VkFormat.R8g8Uint;
        public const VkFormat VK_FORMAT_R8G8_SINT = VkFormat.R8g8Sint;
        public const VkFormat VK_FORMAT_R8G8_SRGB = VkFormat.R8g8Srgb;
        public const VkFormat VK_FORMAT_R8G8B8_UNORM = VkFormat.R8g8b8Unorm;
        public const VkFormat VK_FORMAT_R8G8B8_SNORM = VkFormat.R8g8b8Snorm;
        public const VkFormat VK_FORMAT_R8G8B8_USCALED = VkFormat.R8g8b8Uscaled;
        public const VkFormat VK_FORMAT_R8G8B8_SSCALED = VkFormat.R8g8b8Sscaled;
        public const VkFormat VK_FORMAT_R8G8B8_UINT = VkFormat.R8g8b8Uint;
        public const VkFormat VK_FORMAT_R8G8B8_SINT = VkFormat.R8g8b8Sint;
        public const VkFormat VK_FORMAT_R8G8B8_SRGB = VkFormat.R8g8b8Srgb;
        public const VkFormat VK_FORMAT_B8G8R8_UNORM = VkFormat.B8g8r8Unorm;
        public const VkFormat VK_FORMAT_B8G8R8_SNORM = VkFormat.B8g8r8Snorm;
        public const VkFormat VK_FORMAT_B8G8R8_USCALED = VkFormat.B8g8r8Uscaled;
        public const VkFormat VK_FORMAT_B8G8R8_SSCALED = VkFormat.B8g8r8Sscaled;
        public const VkFormat VK_FORMAT_B8G8R8_UINT = VkFormat.B8g8r8Uint;
        public const VkFormat VK_FORMAT_B8G8R8_SINT = VkFormat.B8g8r8Sint;
        public const VkFormat VK_FORMAT_B8G8R8_SRGB = VkFormat.B8g8r8Srgb;
        public const VkFormat VK_FORMAT_R8G8B8A8_UNORM = VkFormat.R8g8b8a8Unorm;
        public const VkFormat VK_FORMAT_R8G8B8A8_SNORM = VkFormat.R8g8b8a8Snorm;
        public const VkFormat VK_FORMAT_R8G8B8A8_USCALED = VkFormat.R8g8b8a8Uscaled;
        public const VkFormat VK_FORMAT_R8G8B8A8_SSCALED = VkFormat.R8g8b8a8Sscaled;
        public const VkFormat VK_FORMAT_R8G8B8A8_UINT = VkFormat.R8g8b8a8Uint;
        public const VkFormat VK_FORMAT_R8G8B8A8_SINT = VkFormat.R8g8b8a8Sint;
        public const VkFormat VK_FORMAT_R8G8B8A8_SRGB = VkFormat.R8g8b8a8Srgb;
        public const VkFormat VK_FORMAT_B8G8R8A8_UNORM = VkFormat.B8g8r8a8Unorm;
        public const VkFormat VK_FORMAT_B8G8R8A8_SNORM = VkFormat.B8g8r8a8Snorm;
        public const VkFormat VK_FORMAT_B8G8R8A8_USCALED = VkFormat.B8g8r8a8Uscaled;
        public const VkFormat VK_FORMAT_B8G8R8A8_SSCALED = VkFormat.B8g8r8a8Sscaled;
        public const VkFormat VK_FORMAT_B8G8R8A8_UINT = VkFormat.B8g8r8a8Uint;
        public const VkFormat VK_FORMAT_B8G8R8A8_SINT = VkFormat.B8g8r8a8Sint;
        public const VkFormat VK_FORMAT_B8G8R8A8_SRGB = VkFormat.B8g8r8a8Srgb;
        public const VkFormat VK_FORMAT_A8B8G8R8_UNORM_PACK32 = VkFormat.A8b8g8r8UnormPack32;
        public const VkFormat VK_FORMAT_A8B8G8R8_SNORM_PACK32 = VkFormat.A8b8g8r8SnormPack32;
        public const VkFormat VK_FORMAT_A8B8G8R8_USCALED_PACK32 = VkFormat.A8b8g8r8UscaledPack32;
        public const VkFormat VK_FORMAT_A8B8G8R8_SSCALED_PACK32 = VkFormat.A8b8g8r8SscaledPack32;
        public const VkFormat VK_FORMAT_A8B8G8R8_UINT_PACK32 = VkFormat.A8b8g8r8UintPack32;
        public const VkFormat VK_FORMAT_A8B8G8R8_SINT_PACK32 = VkFormat.A8b8g8r8SintPack32;
        public const VkFormat VK_FORMAT_A8B8G8R8_SRGB_PACK32 = VkFormat.A8b8g8r8SrgbPack32;
        public const VkFormat VK_FORMAT_A2R10G10B10_UNORM_PACK32 = VkFormat.A2r10g10b10UnormPack32;
        public const VkFormat VK_FORMAT_A2R10G10B10_SNORM_PACK32 = VkFormat.A2r10g10b10SnormPack32;
        public const VkFormat VK_FORMAT_A2R10G10B10_USCALED_PACK32 = VkFormat.A2r10g10b10UscaledPack32;
        public const VkFormat VK_FORMAT_A2R10G10B10_SSCALED_PACK32 = VkFormat.A2r10g10b10SscaledPack32;
        public const VkFormat VK_FORMAT_A2R10G10B10_UINT_PACK32 = VkFormat.A2r10g10b10UintPack32;
        public const VkFormat VK_FORMAT_A2R10G10B10_SINT_PACK32 = VkFormat.A2r10g10b10SintPack32;
        public const VkFormat VK_FORMAT_A2B10G10R10_UNORM_PACK32 = VkFormat.A2b10g10r10UnormPack32;
        public const VkFormat VK_FORMAT_A2B10G10R10_SNORM_PACK32 = VkFormat.A2b10g10r10SnormPack32;
        public const VkFormat VK_FORMAT_A2B10G10R10_USCALED_PACK32 = VkFormat.A2b10g10r10UscaledPack32;
        public const VkFormat VK_FORMAT_A2B10G10R10_SSCALED_PACK32 = VkFormat.A2b10g10r10SscaledPack32;
        public const VkFormat VK_FORMAT_A2B10G10R10_UINT_PACK32 = VkFormat.A2b10g10r10UintPack32;
        public const VkFormat VK_FORMAT_A2B10G10R10_SINT_PACK32 = VkFormat.A2b10g10r10SintPack32;
        public const VkFormat VK_FORMAT_R16_UNORM = VkFormat.R16Unorm;
        public const VkFormat VK_FORMAT_R16_SNORM = VkFormat.R16Snorm;
        public const VkFormat VK_FORMAT_R16_USCALED = VkFormat.R16Uscaled;
        public const VkFormat VK_FORMAT_R16_SSCALED = VkFormat.R16Sscaled;
        public const VkFormat VK_FORMAT_R16_UINT = VkFormat.R16Uint;
        public const VkFormat VK_FORMAT_R16_SINT = VkFormat.R16Sint;
        public const VkFormat VK_FORMAT_R16_SFLOAT = VkFormat.R16Sfloat;
        public const VkFormat VK_FORMAT_R16G16_UNORM = VkFormat.R16g16Unorm;
        public const VkFormat VK_FORMAT_R16G16_SNORM = VkFormat.R16g16Snorm;
        public const VkFormat VK_FORMAT_R16G16_USCALED = VkFormat.R16g16Uscaled;
        public const VkFormat VK_FORMAT_R16G16_SSCALED = VkFormat.R16g16Sscaled;
        public const VkFormat VK_FORMAT_R16G16_UINT = VkFormat.R16g16Uint;
        public const VkFormat VK_FORMAT_R16G16_SINT = VkFormat.R16g16Sint;
        public const VkFormat VK_FORMAT_R16G16_SFLOAT = VkFormat.R16g16Sfloat;
        public const VkFormat VK_FORMAT_R16G16B16_UNORM = VkFormat.R16g16b16Unorm;
        public const VkFormat VK_FORMAT_R16G16B16_SNORM = VkFormat.R16g16b16Snorm;
        public const VkFormat VK_FORMAT_R16G16B16_USCALED = VkFormat.R16g16b16Uscaled;
        public const VkFormat VK_FORMAT_R16G16B16_SSCALED = VkFormat.R16g16b16Sscaled;
        public const VkFormat VK_FORMAT_R16G16B16_UINT = VkFormat.R16g16b16Uint;
        public const VkFormat VK_FORMAT_R16G16B16_SINT = VkFormat.R16g16b16Sint;
        public const VkFormat VK_FORMAT_R16G16B16_SFLOAT = VkFormat.R16g16b16Sfloat;
        public const VkFormat VK_FORMAT_R16G16B16A16_UNORM = VkFormat.R16g16b16a16Unorm;
        public const VkFormat VK_FORMAT_R16G16B16A16_SNORM = VkFormat.R16g16b16a16Snorm;
        public const VkFormat VK_FORMAT_R16G16B16A16_USCALED = VkFormat.R16g16b16a16Uscaled;
        public const VkFormat VK_FORMAT_R16G16B16A16_SSCALED = VkFormat.R16g16b16a16Sscaled;
        public const VkFormat VK_FORMAT_R16G16B16A16_UINT = VkFormat.R16g16b16a16Uint;
        public const VkFormat VK_FORMAT_R16G16B16A16_SINT = VkFormat.R16g16b16a16Sint;
        public const VkFormat VK_FORMAT_R16G16B16A16_SFLOAT = VkFormat.R16g16b16a16Sfloat;
        public const VkFormat VK_FORMAT_R32_UINT = VkFormat.R32Uint;
        public const VkFormat VK_FORMAT_R32_SINT = VkFormat.R32Sint;
        public const VkFormat VK_FORMAT_R32_SFLOAT = VkFormat.R32Sfloat;
        public const VkFormat VK_FORMAT_R32G32_UINT = VkFormat.R32g32Uint;
        public const VkFormat VK_FORMAT_R32G32_SINT = VkFormat.R32g32Sint;
        public const VkFormat VK_FORMAT_R32G32_SFLOAT = VkFormat.R32g32Sfloat;
        public const VkFormat VK_FORMAT_R32G32B32_UINT = VkFormat.R32g32b32Uint;
        public const VkFormat VK_FORMAT_R32G32B32_SINT = VkFormat.R32g32b32Sint;
        public const VkFormat VK_FORMAT_R32G32B32_SFLOAT = VkFormat.R32g32b32Sfloat;
        public const VkFormat VK_FORMAT_R32G32B32A32_UINT = VkFormat.R32g32b32a32Uint;
        public const VkFormat VK_FORMAT_R32G32B32A32_SINT = VkFormat.R32g32b32a32Sint;
        public const VkFormat VK_FORMAT_R32G32B32A32_SFLOAT = VkFormat.R32g32b32a32Sfloat;
        public const VkFormat VK_FORMAT_R64_UINT = VkFormat.R64Uint;
        public const VkFormat VK_FORMAT_R64_SINT = VkFormat.R64Sint;
        public const VkFormat VK_FORMAT_R64_SFLOAT = VkFormat.R64Sfloat;
        public const VkFormat VK_FORMAT_R64G64_UINT = VkFormat.R64g64Uint;
        public const VkFormat VK_FORMAT_R64G64_SINT = VkFormat.R64g64Sint;
        public const VkFormat VK_FORMAT_R64G64_SFLOAT = VkFormat.R64g64Sfloat;
        public const VkFormat VK_FORMAT_R64G64B64_UINT = VkFormat.R64g64b64Uint;
        public const VkFormat VK_FORMAT_R64G64B64_SINT = VkFormat.R64g64b64Sint;
        public const VkFormat VK_FORMAT_R64G64B64_SFLOAT = VkFormat.R64g64b64Sfloat;
        public const VkFormat VK_FORMAT_R64G64B64A64_UINT = VkFormat.R64g64b64a64Uint;
        public const VkFormat VK_FORMAT_R64G64B64A64_SINT = VkFormat.R64g64b64a64Sint;
        public const VkFormat VK_FORMAT_R64G64B64A64_SFLOAT = VkFormat.R64g64b64a64Sfloat;
        public const VkFormat VK_FORMAT_B10G11R11_UFLOAT_PACK32 = VkFormat.B10g11r11UfloatPack32;
        public const VkFormat VK_FORMAT_E5B9G9R9_UFLOAT_PACK32 = VkFormat.E5b9g9r9UfloatPack32;
        public const VkFormat VK_FORMAT_D16_UNORM = VkFormat.D16Unorm;
        public const VkFormat VK_FORMAT_X8_D24_UNORM_PACK32 = VkFormat.X8D24UnormPack32;
        public const VkFormat VK_FORMAT_D32_SFLOAT = VkFormat.D32Sfloat;
        public const VkFormat VK_FORMAT_S8_UINT = VkFormat.S8Uint;
        public const VkFormat VK_FORMAT_D16_UNORM_S8_UINT = VkFormat.D16UnormS8Uint;
        public const VkFormat VK_FORMAT_D24_UNORM_S8_UINT = VkFormat.D24UnormS8Uint;
        public const VkFormat VK_FORMAT_D32_SFLOAT_S8_UINT = VkFormat.D32SfloatS8Uint;
        public const VkFormat VK_FORMAT_BC1_RGB_UNORM_BLOCK = VkFormat.Bc1RgbUnormBlock;
        public const VkFormat VK_FORMAT_BC1_RGB_SRGB_BLOCK = VkFormat.Bc1RgbSrgbBlock;
        public const VkFormat VK_FORMAT_BC1_RGBA_UNORM_BLOCK = VkFormat.Bc1RgbaUnormBlock;
        public const VkFormat VK_FORMAT_BC1_RGBA_SRGB_BLOCK = VkFormat.Bc1RgbaSrgbBlock;
        public const VkFormat VK_FORMAT_BC2_UNORM_BLOCK = VkFormat.Bc2UnormBlock;
        public const VkFormat VK_FORMAT_BC2_SRGB_BLOCK = VkFormat.Bc2SrgbBlock;
        public const VkFormat VK_FORMAT_BC3_UNORM_BLOCK = VkFormat.Bc3UnormBlock;
        public const VkFormat VK_FORMAT_BC3_SRGB_BLOCK = VkFormat.Bc3SrgbBlock;
        public const VkFormat VK_FORMAT_BC4_UNORM_BLOCK = VkFormat.Bc4UnormBlock;
        public const VkFormat VK_FORMAT_BC4_SNORM_BLOCK = VkFormat.Bc4SnormBlock;
        public const VkFormat VK_FORMAT_BC5_UNORM_BLOCK = VkFormat.Bc5UnormBlock;
        public const VkFormat VK_FORMAT_BC5_SNORM_BLOCK = VkFormat.Bc5SnormBlock;
        public const VkFormat VK_FORMAT_BC6H_UFLOAT_BLOCK = VkFormat.Bc6hUfloatBlock;
        public const VkFormat VK_FORMAT_BC6H_SFLOAT_BLOCK = VkFormat.Bc6hSfloatBlock;
        public const VkFormat VK_FORMAT_BC7_UNORM_BLOCK = VkFormat.Bc7UnormBlock;
        public const VkFormat VK_FORMAT_BC7_SRGB_BLOCK = VkFormat.Bc7SrgbBlock;
        public const VkFormat VK_FORMAT_ETC2_R8G8B8_UNORM_BLOCK = VkFormat.Etc2R8g8b8UnormBlock;
        public const VkFormat VK_FORMAT_ETC2_R8G8B8_SRGB_BLOCK = VkFormat.Etc2R8g8b8SrgbBlock;
        public const VkFormat VK_FORMAT_ETC2_R8G8B8A1_UNORM_BLOCK = VkFormat.Etc2R8g8b8a1UnormBlock;
        public const VkFormat VK_FORMAT_ETC2_R8G8B8A1_SRGB_BLOCK = VkFormat.Etc2R8g8b8a1SrgbBlock;
        public const VkFormat VK_FORMAT_ETC2_R8G8B8A8_UNORM_BLOCK = VkFormat.Etc2R8g8b8a8UnormBlock;
        public const VkFormat VK_FORMAT_ETC2_R8G8B8A8_SRGB_BLOCK = VkFormat.Etc2R8g8b8a8SrgbBlock;
        public const VkFormat VK_FORMAT_EAC_R11_UNORM_BLOCK = VkFormat.EacR11UnormBlock;
        public const VkFormat VK_FORMAT_EAC_R11_SNORM_BLOCK = VkFormat.EacR11SnormBlock;
        public const VkFormat VK_FORMAT_EAC_R11G11_UNORM_BLOCK = VkFormat.EacR11g11UnormBlock;
        public const VkFormat VK_FORMAT_EAC_R11G11_SNORM_BLOCK = VkFormat.EacR11g11SnormBlock;
        public const VkFormat VK_FORMAT_ASTC_4x4_UNORM_BLOCK = VkFormat.Astc4x4UnormBlock;
        public const VkFormat VK_FORMAT_ASTC_4x4_SRGB_BLOCK = VkFormat.Astc4x4SrgbBlock;
        public const VkFormat VK_FORMAT_ASTC_5x4_UNORM_BLOCK = VkFormat.Astc5x4UnormBlock;
        public const VkFormat VK_FORMAT_ASTC_5x4_SRGB_BLOCK = VkFormat.Astc5x4SrgbBlock;
        public const VkFormat VK_FORMAT_ASTC_5x5_UNORM_BLOCK = VkFormat.Astc5x5UnormBlock;
        public const VkFormat VK_FORMAT_ASTC_5x5_SRGB_BLOCK = VkFormat.Astc5x5SrgbBlock;
        public const VkFormat VK_FORMAT_ASTC_6x5_UNORM_BLOCK = VkFormat.Astc6x5UnormBlock;
        public const VkFormat VK_FORMAT_ASTC_6x5_SRGB_BLOCK = VkFormat.Astc6x5SrgbBlock;
        public const VkFormat VK_FORMAT_ASTC_6x6_UNORM_BLOCK = VkFormat.Astc6x6UnormBlock;
        public const VkFormat VK_FORMAT_ASTC_6x6_SRGB_BLOCK = VkFormat.Astc6x6SrgbBlock;
        public const VkFormat VK_FORMAT_ASTC_8x5_UNORM_BLOCK = VkFormat.Astc8x5UnormBlock;
        public const VkFormat VK_FORMAT_ASTC_8x5_SRGB_BLOCK = VkFormat.Astc8x5SrgbBlock;
        public const VkFormat VK_FORMAT_ASTC_8x6_UNORM_BLOCK = VkFormat.Astc8x6UnormBlock;
        public const VkFormat VK_FORMAT_ASTC_8x6_SRGB_BLOCK = VkFormat.Astc8x6SrgbBlock;
        public const VkFormat VK_FORMAT_ASTC_8x8_UNORM_BLOCK = VkFormat.Astc8x8UnormBlock;
        public const VkFormat VK_FORMAT_ASTC_8x8_SRGB_BLOCK = VkFormat.Astc8x8SrgbBlock;
        public const VkFormat VK_FORMAT_ASTC_10x5_UNORM_BLOCK = VkFormat.Astc10x5UnormBlock;
        public const VkFormat VK_FORMAT_ASTC_10x5_SRGB_BLOCK = VkFormat.Astc10x5SrgbBlock;
        public const VkFormat VK_FORMAT_ASTC_10x6_UNORM_BLOCK = VkFormat.Astc10x6UnormBlock;
        public const VkFormat VK_FORMAT_ASTC_10x6_SRGB_BLOCK = VkFormat.Astc10x6SrgbBlock;
        public const VkFormat VK_FORMAT_ASTC_10x8_UNORM_BLOCK = VkFormat.Astc10x8UnormBlock;
        public const VkFormat VK_FORMAT_ASTC_10x8_SRGB_BLOCK = VkFormat.Astc10x8SrgbBlock;
        public const VkFormat VK_FORMAT_ASTC_10x10_UNORM_BLOCK = VkFormat.Astc10x10UnormBlock;
        public const VkFormat VK_FORMAT_ASTC_10x10_SRGB_BLOCK = VkFormat.Astc10x10SrgbBlock;
        public const VkFormat VK_FORMAT_ASTC_12x10_UNORM_BLOCK = VkFormat.Astc12x10UnormBlock;
        public const VkFormat VK_FORMAT_ASTC_12x10_SRGB_BLOCK = VkFormat.Astc12x10SrgbBlock;
        public const VkFormat VK_FORMAT_ASTC_12x12_UNORM_BLOCK = VkFormat.Astc12x12UnormBlock;
        public const VkFormat VK_FORMAT_ASTC_12x12_SRGB_BLOCK = VkFormat.Astc12x12SrgbBlock;
        public const VkFormat VK_FORMAT_PVRTC1_2BPP_UNORM_BLOCK_IMG = VkFormat.Pvrtc12bppUnormBlockImg;
        public const VkFormat VK_FORMAT_PVRTC1_4BPP_UNORM_BLOCK_IMG = VkFormat.Pvrtc14bppUnormBlockImg;
        public const VkFormat VK_FORMAT_PVRTC2_2BPP_UNORM_BLOCK_IMG = VkFormat.Pvrtc22bppUnormBlockImg;
        public const VkFormat VK_FORMAT_PVRTC2_4BPP_UNORM_BLOCK_IMG = VkFormat.Pvrtc24bppUnormBlockImg;
        public const VkFormat VK_FORMAT_PVRTC1_2BPP_SRGB_BLOCK_IMG = VkFormat.Pvrtc12bppSrgbBlockImg;
        public const VkFormat VK_FORMAT_PVRTC1_4BPP_SRGB_BLOCK_IMG = VkFormat.Pvrtc14bppSrgbBlockImg;
        public const VkFormat VK_FORMAT_PVRTC2_2BPP_SRGB_BLOCK_IMG = VkFormat.Pvrtc22bppSrgbBlockImg;
        public const VkFormat VK_FORMAT_PVRTC2_4BPP_SRGB_BLOCK_IMG = VkFormat.Pvrtc24bppSrgbBlockImg;
    }

    public enum VkStructureType
    {
        ApplicationInfo = 0,
        InstanceCreateInfo = 1,
        DeviceQueueCreateInfo = 2,
        DeviceCreateInfo = 3,
        SubmitInfo = 4,
        MemoryAllocateInfo = 5,
        MappedMemoryRange = 6,
        BindSparseInfo = 7,
        FenceCreateInfo = 8,
        SemaphoreCreateInfo = 9,
        EventCreateInfo = 10,
        QueryPoolCreateInfo = 11,
        BufferCreateInfo = 12,
        BufferViewCreateInfo = 13,
        ImageCreateInfo = 14,
        ImageViewCreateInfo = 15,
        ShaderModuleCreateInfo = 16,
        PipelineCacheCreateInfo = 17,
        PipelineShaderStageCreateInfo = 18,
        PipelineVertexInputStateCreateInfo = 19,
        PipelineInputAssemblyStateCreateInfo = 20,
        PipelineTessellationStateCreateInfo = 21,
        PipelineViewportStateCreateInfo = 22,
        PipelineRasterizationStateCreateInfo = 23,
        PipelineMultisampleStateCreateInfo = 24,
        PipelineDepthStencilStateCreateInfo = 25,
        PipelineColorBlendStateCreateInfo = 26,
        PipelineDynamicStateCreateInfo = 27,
        GraphicsPipelineCreateInfo = 28,
        ComputePipelineCreateInfo = 29,
        PipelineLayoutCreateInfo = 30,
        SamplerCreateInfo = 31,
        DescriptorSetLayoutCreateInfo = 32,
        DescriptorPoolCreateInfo = 33,
        DescriptorSetAllocateInfo = 34,
        WriteDescriptorSet = 35,
        CopyDescriptorSet = 36,
        FramebufferCreateInfo = 37,
        RenderPassCreateInfo = 38,
        CommandPoolCreateInfo = 39,
        CommandBufferAllocateInfo = 40,
        CommandBufferInheritanceInfo = 41,
        CommandBufferBeginInfo = 42,
        RenderPassBeginInfo = 43,
        BufferMemoryBarrier = 44,
        ImageMemoryBarrier = 45,
        MemoryBarrier = 46,
        LoaderInstanceCreateInfo = 47,
        LoaderDeviceCreateInfo = 48,
        SwapchainCreateInfo = 1000001000,
        PresentInfo = 1000001001,
        DisplayModeCreateInfo = 1000002000,
        DisplaySurfaceCreateInfo = 1000002001,
        DisplayPresentInfo = 1000003000,
        XlibSurfaceCreateInfo = 1000004000,
        XcbSurfaceCreateInfo = 1000005000,
        WaylandSurfaceCreateInfo = 1000006000,
        MirSurfaceCreateInfo = 1000007000,
        AndroidSurfaceCreateInfo = 1000008000,
        Win32SurfaceCreateInfo = 1000009000,
        DebugReportCallbackCreateInfo = 1000011000,
        PipelineRasterizationStateRasterizationOrder = 1000018000,
        DebugMarkerObjectNameInfo = 1000022000,
        DebugMarkerObjectTagInfo = 1000022001,
        DebugMarkerMarkerInfo = 1000022002,
        DedicatedAllocationImageCreateInfo = 1000026000,
        DedicatedAllocationBufferCreateInfo = 1000026001,
        DedicatedAllocationMemoryAllocateInfo = 1000026002,
        ExternalMemoryImageCreateInfo = 1000056000,
        ExportMemoryAllocateInfo = 1000056001,
        ImportMemoryWin32HandleInfo = 1000057000,
        ExportMemoryWin32HandleInfo = 1000057001,
        Win32KeyedMutexAcquireReleaseInfo = 1000058000,
        Validation = 1000061000,
        ObjectTableCreateInfo = 1000086000,
        IndirectCommandsLayoutCreateInfo = 1000086001,
        CmdProcessCommandsInfo = 1000086002,
        CmdReserveSpaceForCommandsInfo = 1000086003,
        DeviceGeneratedCommandsLimits = 1000086004,
        DeviceGeneratedCommandsFeatures = 1000086005,
    }
    public static partial class RawConstants
    {
        public const VkStructureType VK_STRUCTURE_TYPE_APPLICATION_INFO = VkStructureType.ApplicationInfo;
        public const VkStructureType VK_STRUCTURE_TYPE_INSTANCE_CREATE_INFO = VkStructureType.InstanceCreateInfo;
        public const VkStructureType VK_STRUCTURE_TYPE_DEVICE_QUEUE_CREATE_INFO = VkStructureType.DeviceQueueCreateInfo;
        public const VkStructureType VK_STRUCTURE_TYPE_DEVICE_CREATE_INFO = VkStructureType.DeviceCreateInfo;
        public const VkStructureType VK_STRUCTURE_TYPE_SUBMIT_INFO = VkStructureType.SubmitInfo;
        public const VkStructureType VK_STRUCTURE_TYPE_MEMORY_ALLOCATE_INFO = VkStructureType.MemoryAllocateInfo;
        public const VkStructureType VK_STRUCTURE_TYPE_MAPPED_MEMORY_RANGE = VkStructureType.MappedMemoryRange;
        public const VkStructureType VK_STRUCTURE_TYPE_BIND_SPARSE_INFO = VkStructureType.BindSparseInfo;
        public const VkStructureType VK_STRUCTURE_TYPE_FENCE_CREATE_INFO = VkStructureType.FenceCreateInfo;
        public const VkStructureType VK_STRUCTURE_TYPE_SEMAPHORE_CREATE_INFO = VkStructureType.SemaphoreCreateInfo;
        public const VkStructureType VK_STRUCTURE_TYPE_EVENT_CREATE_INFO = VkStructureType.EventCreateInfo;
        public const VkStructureType VK_STRUCTURE_TYPE_QUERY_POOL_CREATE_INFO = VkStructureType.QueryPoolCreateInfo;
        public const VkStructureType VK_STRUCTURE_TYPE_BUFFER_CREATE_INFO = VkStructureType.BufferCreateInfo;
        public const VkStructureType VK_STRUCTURE_TYPE_BUFFER_VIEW_CREATE_INFO = VkStructureType.BufferViewCreateInfo;
        public const VkStructureType VK_STRUCTURE_TYPE_IMAGE_CREATE_INFO = VkStructureType.ImageCreateInfo;
        public const VkStructureType VK_STRUCTURE_TYPE_IMAGE_VIEW_CREATE_INFO = VkStructureType.ImageViewCreateInfo;
        public const VkStructureType VK_STRUCTURE_TYPE_SHADER_MODULE_CREATE_INFO = VkStructureType.ShaderModuleCreateInfo;
        public const VkStructureType VK_STRUCTURE_TYPE_PIPELINE_CACHE_CREATE_INFO = VkStructureType.PipelineCacheCreateInfo;
        public const VkStructureType VK_STRUCTURE_TYPE_PIPELINE_SHADER_STAGE_CREATE_INFO = VkStructureType.PipelineShaderStageCreateInfo;
        public const VkStructureType VK_STRUCTURE_TYPE_PIPELINE_VERTEX_INPUT_STATE_CREATE_INFO = VkStructureType.PipelineVertexInputStateCreateInfo;
        public const VkStructureType VK_STRUCTURE_TYPE_PIPELINE_INPUT_ASSEMBLY_STATE_CREATE_INFO = VkStructureType.PipelineInputAssemblyStateCreateInfo;
        public const VkStructureType VK_STRUCTURE_TYPE_PIPELINE_TESSELLATION_STATE_CREATE_INFO = VkStructureType.PipelineTessellationStateCreateInfo;
        public const VkStructureType VK_STRUCTURE_TYPE_PIPELINE_VIEWPORT_STATE_CREATE_INFO = VkStructureType.PipelineViewportStateCreateInfo;
        public const VkStructureType VK_STRUCTURE_TYPE_PIPELINE_RASTERIZATION_STATE_CREATE_INFO = VkStructureType.PipelineRasterizationStateCreateInfo;
        public const VkStructureType VK_STRUCTURE_TYPE_PIPELINE_MULTISAMPLE_STATE_CREATE_INFO = VkStructureType.PipelineMultisampleStateCreateInfo;
        public const VkStructureType VK_STRUCTURE_TYPE_PIPELINE_DEPTH_STENCIL_STATE_CREATE_INFO = VkStructureType.PipelineDepthStencilStateCreateInfo;
        public const VkStructureType VK_STRUCTURE_TYPE_PIPELINE_COLOR_BLEND_STATE_CREATE_INFO = VkStructureType.PipelineColorBlendStateCreateInfo;
        public const VkStructureType VK_STRUCTURE_TYPE_PIPELINE_DYNAMIC_STATE_CREATE_INFO = VkStructureType.PipelineDynamicStateCreateInfo;
        public const VkStructureType VK_STRUCTURE_TYPE_GRAPHICS_PIPELINE_CREATE_INFO = VkStructureType.GraphicsPipelineCreateInfo;
        public const VkStructureType VK_STRUCTURE_TYPE_COMPUTE_PIPELINE_CREATE_INFO = VkStructureType.ComputePipelineCreateInfo;
        public const VkStructureType VK_STRUCTURE_TYPE_PIPELINE_LAYOUT_CREATE_INFO = VkStructureType.PipelineLayoutCreateInfo;
        public const VkStructureType VK_STRUCTURE_TYPE_SAMPLER_CREATE_INFO = VkStructureType.SamplerCreateInfo;
        public const VkStructureType VK_STRUCTURE_TYPE_DESCRIPTOR_SET_LAYOUT_CREATE_INFO = VkStructureType.DescriptorSetLayoutCreateInfo;
        public const VkStructureType VK_STRUCTURE_TYPE_DESCRIPTOR_POOL_CREATE_INFO = VkStructureType.DescriptorPoolCreateInfo;
        public const VkStructureType VK_STRUCTURE_TYPE_DESCRIPTOR_SET_ALLOCATE_INFO = VkStructureType.DescriptorSetAllocateInfo;
        public const VkStructureType VK_STRUCTURE_TYPE_WRITE_DESCRIPTOR_SET = VkStructureType.WriteDescriptorSet;
        public const VkStructureType VK_STRUCTURE_TYPE_COPY_DESCRIPTOR_SET = VkStructureType.CopyDescriptorSet;
        public const VkStructureType VK_STRUCTURE_TYPE_FRAMEBUFFER_CREATE_INFO = VkStructureType.FramebufferCreateInfo;
        public const VkStructureType VK_STRUCTURE_TYPE_RENDER_PASS_CREATE_INFO = VkStructureType.RenderPassCreateInfo;
        public const VkStructureType VK_STRUCTURE_TYPE_COMMAND_POOL_CREATE_INFO = VkStructureType.CommandPoolCreateInfo;
        public const VkStructureType VK_STRUCTURE_TYPE_COMMAND_BUFFER_ALLOCATE_INFO = VkStructureType.CommandBufferAllocateInfo;
        public const VkStructureType VK_STRUCTURE_TYPE_COMMAND_BUFFER_INHERITANCE_INFO = VkStructureType.CommandBufferInheritanceInfo;
        public const VkStructureType VK_STRUCTURE_TYPE_COMMAND_BUFFER_BEGIN_INFO = VkStructureType.CommandBufferBeginInfo;
        public const VkStructureType VK_STRUCTURE_TYPE_RENDER_PASS_BEGIN_INFO = VkStructureType.RenderPassBeginInfo;
        public const VkStructureType VK_STRUCTURE_TYPE_BUFFER_MEMORY_BARRIER = VkStructureType.BufferMemoryBarrier;
        public const VkStructureType VK_STRUCTURE_TYPE_IMAGE_MEMORY_BARRIER = VkStructureType.ImageMemoryBarrier;
        public const VkStructureType VK_STRUCTURE_TYPE_MEMORY_BARRIER = VkStructureType.MemoryBarrier;
        public const VkStructureType VK_STRUCTURE_TYPE_LOADER_INSTANCE_CREATE_INFO = VkStructureType.LoaderInstanceCreateInfo;
        public const VkStructureType VK_STRUCTURE_TYPE_LOADER_DEVICE_CREATE_INFO = VkStructureType.LoaderDeviceCreateInfo;
        public const VkStructureType VK_STRUCTURE_TYPE_SWAPCHAIN_CREATE_INFO_KHR = VkStructureType.SwapchainCreateInfo;
        public const VkStructureType VK_STRUCTURE_TYPE_PRESENT_INFO_KHR = VkStructureType.PresentInfo;
        public const VkStructureType VK_STRUCTURE_TYPE_DISPLAY_MODE_CREATE_INFO_KHR = VkStructureType.DisplayModeCreateInfo;
        public const VkStructureType VK_STRUCTURE_TYPE_DISPLAY_SURFACE_CREATE_INFO_KHR = VkStructureType.DisplaySurfaceCreateInfo;
        public const VkStructureType VK_STRUCTURE_TYPE_DISPLAY_PRESENT_INFO_KHR = VkStructureType.DisplayPresentInfo;
        public const VkStructureType VK_STRUCTURE_TYPE_XLIB_SURFACE_CREATE_INFO_KHR = VkStructureType.XlibSurfaceCreateInfo;
        public const VkStructureType VK_STRUCTURE_TYPE_XCB_SURFACE_CREATE_INFO_KHR = VkStructureType.XcbSurfaceCreateInfo;
        public const VkStructureType VK_STRUCTURE_TYPE_WAYLAND_SURFACE_CREATE_INFO_KHR = VkStructureType.WaylandSurfaceCreateInfo;
        public const VkStructureType VK_STRUCTURE_TYPE_MIR_SURFACE_CREATE_INFO_KHR = VkStructureType.MirSurfaceCreateInfo;
        public const VkStructureType VK_STRUCTURE_TYPE_ANDROID_SURFACE_CREATE_INFO_KHR = VkStructureType.AndroidSurfaceCreateInfo;
        public const VkStructureType VK_STRUCTURE_TYPE_WIN32_SURFACE_CREATE_INFO_KHR = VkStructureType.Win32SurfaceCreateInfo;
        public const VkStructureType VK_STRUCTURE_TYPE_DEBUG_REPORT_CALLBACK_CREATE_INFO_EXT = VkStructureType.DebugReportCallbackCreateInfo;
        public const VkStructureType VK_STRUCTURE_TYPE_PIPELINE_RASTERIZATION_STATE_RASTERIZATION_ORDER_AMD = VkStructureType.PipelineRasterizationStateRasterizationOrder;
        public const VkStructureType VK_STRUCTURE_TYPE_DEBUG_MARKER_OBJECT_NAME_INFO_EXT = VkStructureType.DebugMarkerObjectNameInfo;
        public const VkStructureType VK_STRUCTURE_TYPE_DEBUG_MARKER_OBJECT_TAG_INFO_EXT = VkStructureType.DebugMarkerObjectTagInfo;
        public const VkStructureType VK_STRUCTURE_TYPE_DEBUG_MARKER_MARKER_INFO_EXT = VkStructureType.DebugMarkerMarkerInfo;
        public const VkStructureType VK_STRUCTURE_TYPE_DEDICATED_ALLOCATION_IMAGE_CREATE_INFO_NV = VkStructureType.DedicatedAllocationImageCreateInfo;
        public const VkStructureType VK_STRUCTURE_TYPE_DEDICATED_ALLOCATION_BUFFER_CREATE_INFO_NV = VkStructureType.DedicatedAllocationBufferCreateInfo;
        public const VkStructureType VK_STRUCTURE_TYPE_DEDICATED_ALLOCATION_MEMORY_ALLOCATE_INFO_NV = VkStructureType.DedicatedAllocationMemoryAllocateInfo;
        public const VkStructureType VK_STRUCTURE_TYPE_EXTERNAL_MEMORY_IMAGE_CREATE_INFO_NV = VkStructureType.ExternalMemoryImageCreateInfo;
        public const VkStructureType VK_STRUCTURE_TYPE_EXPORT_MEMORY_ALLOCATE_INFO_NV = VkStructureType.ExportMemoryAllocateInfo;
        public const VkStructureType VK_STRUCTURE_TYPE_IMPORT_MEMORY_WIN32_HANDLE_INFO_NV = VkStructureType.ImportMemoryWin32HandleInfo;
        public const VkStructureType VK_STRUCTURE_TYPE_EXPORT_MEMORY_WIN32_HANDLE_INFO_NV = VkStructureType.ExportMemoryWin32HandleInfo;
        public const VkStructureType VK_STRUCTURE_TYPE_WIN32_KEYED_MUTEX_ACQUIRE_RELEASE_INFO_NV = VkStructureType.Win32KeyedMutexAcquireReleaseInfo;
        public const VkStructureType VK_STRUCTURE_TYPE_VALIDATION_FLAGS_EXT = VkStructureType.Validation;
        public const VkStructureType VK_STRUCTURE_TYPE_OBJECT_TABLE_CREATE_INFO_NVX = VkStructureType.ObjectTableCreateInfo;
        public const VkStructureType VK_STRUCTURE_TYPE_INDIRECT_COMMANDS_LAYOUT_CREATE_INFO_NVX = VkStructureType.IndirectCommandsLayoutCreateInfo;
        public const VkStructureType VK_STRUCTURE_TYPE_CMD_PROCESS_COMMANDS_INFO_NVX = VkStructureType.CmdProcessCommandsInfo;
        public const VkStructureType VK_STRUCTURE_TYPE_CMD_RESERVE_SPACE_FOR_COMMANDS_INFO_NVX = VkStructureType.CmdReserveSpaceForCommandsInfo;
        public const VkStructureType VK_STRUCTURE_TYPE_DEVICE_GENERATED_COMMANDS_LIMITS_NVX = VkStructureType.DeviceGeneratedCommandsLimits;
        public const VkStructureType VK_STRUCTURE_TYPE_DEVICE_GENERATED_COMMANDS_FEATURES_NVX = VkStructureType.DeviceGeneratedCommandsFeatures;
    }

    public enum VkSubpassContents
    {
        Inline = 0,
        SecondaryCommandBuffers = 1,
    }
    public static partial class RawConstants
    {
        public const VkSubpassContents VK_SUBPASS_CONTENTS_INLINE = VkSubpassContents.Inline;
        public const VkSubpassContents VK_SUBPASS_CONTENTS_SECONDARY_COMMAND_BUFFERS = VkSubpassContents.SecondaryCommandBuffers;
    }

    public enum VkResult
    {
        ///<summary>Command completed successfully</summary>
        Success = 0,
        ///<summary>A fence or query has not yet completed</summary>
        NotReady = 1,
        ///<summary>A wait operation has not completed in the specified time</summary>
        Timeout = 2,
        ///<summary>An event is signaled</summary>
        EventSet = 3,
        ///<summary>An event is unsignaled</summary>
        EventReset = 4,
        ///<summary>A return array was too small for the result</summary>
        Incomplete = 5,
        ///<summary>A host memory allocation has failed</summary>
        ErrorOutOfHostMemory = -1,
        ///<summary>A device memory allocation has failed</summary>
        ErrorOutOfDeviceMemory = -2,
        ///<summary>Initialization of a object has failed</summary>
        ErrorInitializationFailed = -3,
        ///<summary>The logical device has been lost. See <<devsandqueues-lost-device>></summary>
        ErrorDeviceLost = -4,
        ///<summary>Mapping of a memory object has failed</summary>
        ErrorMemoryMapFailed = -5,
        ///<summary>Layer specified does not exist</summary>
        ErrorLayerNotPresent = -6,
        ///<summary>Extension specified does not exist</summary>
        ErrorExtensionNotPresent = -7,
        ///<summary>Requested feature is not available on this device</summary>
        ErrorFeatureNotPresent = -8,
        ///<summary>Unable to find a Vulkan driver</summary>
        ErrorIncompatibleDriver = -9,
        ///<summary>Too many objects of the type have already been created</summary>
        ErrorTooManyObjects = -10,
        ///<summary>Requested format is not supported on this device</summary>
        ErrorFormatNotSupported = -11,
        ///<summary>A requested pool allocation has failed due to fragmentation of the pool's memory</summary>
        ErrorFragmentedPool = -12,
        ErrorSurfaceLost = 1000000000,
        ErrorNativeWindowInUse = 1000000001,
        Suboptimal = 1000001003,
        ErrorOutOfDate = 1000001004,
        ErrorIncompatibleDisplay = 1000003001,
        ErrorValidationFailed = 1000011001,
        ErrorInvalidShader = 1000012000,
        Extension1Error = 1000013000,
    }
    public static partial class RawConstants
    {
        ///<summary>Command completed successfully</summary>
        public const VkResult VK_SUCCESS = VkResult.Success;
        ///<summary>A fence or query has not yet completed</summary>
        public const VkResult VK_NOT_READY = VkResult.NotReady;
        ///<summary>A wait operation has not completed in the specified time</summary>
        public const VkResult VK_TIMEOUT = VkResult.Timeout;
        ///<summary>An event is signaled</summary>
        public const VkResult VK_EVENT_SET = VkResult.EventSet;
        ///<summary>An event is unsignaled</summary>
        public const VkResult VK_EVENT_RESET = VkResult.EventReset;
        ///<summary>A return array was too small for the result</summary>
        public const VkResult VK_INCOMPLETE = VkResult.Incomplete;
        ///<summary>A host memory allocation has failed</summary>
        public const VkResult VK_ERROR_OUT_OF_HOST_MEMORY = VkResult.ErrorOutOfHostMemory;
        ///<summary>A device memory allocation has failed</summary>
        public const VkResult VK_ERROR_OUT_OF_DEVICE_MEMORY = VkResult.ErrorOutOfDeviceMemory;
        ///<summary>Initialization of a object has failed</summary>
        public const VkResult VK_ERROR_INITIALIZATION_FAILED = VkResult.ErrorInitializationFailed;
        ///<summary>The logical device has been lost. See <<devsandqueues-lost-device>></summary>
        public const VkResult VK_ERROR_DEVICE_LOST = VkResult.ErrorDeviceLost;
        ///<summary>Mapping of a memory object has failed</summary>
        public const VkResult VK_ERROR_MEMORY_MAP_FAILED = VkResult.ErrorMemoryMapFailed;
        ///<summary>Layer specified does not exist</summary>
        public const VkResult VK_ERROR_LAYER_NOT_PRESENT = VkResult.ErrorLayerNotPresent;
        ///<summary>Extension specified does not exist</summary>
        public const VkResult VK_ERROR_EXTENSION_NOT_PRESENT = VkResult.ErrorExtensionNotPresent;
        ///<summary>Requested feature is not available on this device</summary>
        public const VkResult VK_ERROR_FEATURE_NOT_PRESENT = VkResult.ErrorFeatureNotPresent;
        ///<summary>Unable to find a Vulkan driver</summary>
        public const VkResult VK_ERROR_INCOMPATIBLE_DRIVER = VkResult.ErrorIncompatibleDriver;
        ///<summary>Too many objects of the type have already been created</summary>
        public const VkResult VK_ERROR_TOO_MANY_OBJECTS = VkResult.ErrorTooManyObjects;
        ///<summary>Requested format is not supported on this device</summary>
        public const VkResult VK_ERROR_FORMAT_NOT_SUPPORTED = VkResult.ErrorFormatNotSupported;
        ///<summary>A requested pool allocation has failed due to fragmentation of the pool's memory</summary>
        public const VkResult VK_ERROR_FRAGMENTED_POOL = VkResult.ErrorFragmentedPool;
        public const VkResult VK_ERROR_SURFACE_LOST_KHR = VkResult.ErrorSurfaceLost;
        public const VkResult VK_ERROR_NATIVE_WINDOW_IN_USE_KHR = VkResult.ErrorNativeWindowInUse;
        public const VkResult VK_SUBOPTIMAL_KHR = VkResult.Suboptimal;
        public const VkResult VK_ERROR_OUT_OF_DATE_KHR = VkResult.ErrorOutOfDate;
        public const VkResult VK_ERROR_INCOMPATIBLE_DISPLAY_KHR = VkResult.ErrorIncompatibleDisplay;
        public const VkResult VK_ERROR_VALIDATION_FAILED_EXT = VkResult.ErrorValidationFailed;
        public const VkResult VK_ERROR_INVALID_SHADER_NV = VkResult.ErrorInvalidShader;
        public const VkResult VK_NV_EXTENSION_1_ERROR = VkResult.Extension1Error;
    }

    public enum VkDynamicState
    {
        Viewport = 0,
        Scissor = 1,
        LineWidth = 2,
        DepthBias = 3,
        BlendConstants = 4,
        DepthBounds = 5,
        StencilCompareMask = 6,
        StencilWriteMask = 7,
        StencilReference = 8,
    }
    public static partial class RawConstants
    {
        public const VkDynamicState VK_DYNAMIC_STATE_VIEWPORT = VkDynamicState.Viewport;
        public const VkDynamicState VK_DYNAMIC_STATE_SCISSOR = VkDynamicState.Scissor;
        public const VkDynamicState VK_DYNAMIC_STATE_LINE_WIDTH = VkDynamicState.LineWidth;
        public const VkDynamicState VK_DYNAMIC_STATE_DEPTH_BIAS = VkDynamicState.DepthBias;
        public const VkDynamicState VK_DYNAMIC_STATE_BLEND_CONSTANTS = VkDynamicState.BlendConstants;
        public const VkDynamicState VK_DYNAMIC_STATE_DEPTH_BOUNDS = VkDynamicState.DepthBounds;
        public const VkDynamicState VK_DYNAMIC_STATE_STENCIL_COMPARE_MASK = VkDynamicState.StencilCompareMask;
        public const VkDynamicState VK_DYNAMIC_STATE_STENCIL_WRITE_MASK = VkDynamicState.StencilWriteMask;
        public const VkDynamicState VK_DYNAMIC_STATE_STENCIL_REFERENCE = VkDynamicState.StencilReference;
    }

    [Flags]
    public enum VkQueueFlags
    {
        None = 0,
        ///<summary>Queue supports graphics operations</summary>
        Graphics = 1,
        ///<summary>Queue supports compute operations</summary>
        Compute = 2,
        ///<summary>Queue supports transfer operations</summary>
        Transfer = 4,
        ///<summary>Queue supports sparse resource memory management operations</summary>
        SparseBinding = 8,
    }
    public static partial class RawConstants
    {
        ///<summary>Queue supports graphics operations</summary>
        public const VkQueueFlags VK_QUEUE_GRAPHICS_BIT = VkQueueFlags.Graphics;
        ///<summary>Queue supports compute operations</summary>
        public const VkQueueFlags VK_QUEUE_COMPUTE_BIT = VkQueueFlags.Compute;
        ///<summary>Queue supports transfer operations</summary>
        public const VkQueueFlags VK_QUEUE_TRANSFER_BIT = VkQueueFlags.Transfer;
        ///<summary>Queue supports sparse resource memory management operations</summary>
        public const VkQueueFlags VK_QUEUE_SPARSE_BINDING_BIT = VkQueueFlags.SparseBinding;
    }

    [Flags]
    public enum VkMemoryPropertyFlags
    {
        None = 0,
        ///<summary>If otherwise stated, then allocate memory on device</summary>
        DeviceLocal = 1,
        ///<summary>Memory is mappable by host</summary>
        HostVisible = 2,
        ///<summary>Memory will have i/o coherency. If not set, application may need to use vkFlushMappedMemoryRanges and vkInvalidateMappedMemoryRanges to flush/invalidate host cache</summary>
        HostCoherent = 4,
        ///<summary>Memory will be cached by the host</summary>
        HostCached = 8,
        ///<summary>Memory may be allocated by the driver when it is required</summary>
        LazilyAllocated = 16,
    }
    public static partial class RawConstants
    {
        ///<summary>If otherwise stated, then allocate memory on device</summary>
        public const VkMemoryPropertyFlags VK_MEMORY_PROPERTY_DEVICE_LOCAL_BIT = VkMemoryPropertyFlags.DeviceLocal;
        ///<summary>Memory is mappable by host</summary>
        public const VkMemoryPropertyFlags VK_MEMORY_PROPERTY_HOST_VISIBLE_BIT = VkMemoryPropertyFlags.HostVisible;
        ///<summary>Memory will have i/o coherency. If not set, application may need to use vkFlushMappedMemoryRanges and vkInvalidateMappedMemoryRanges to flush/invalidate host cache</summary>
        public const VkMemoryPropertyFlags VK_MEMORY_PROPERTY_HOST_COHERENT_BIT = VkMemoryPropertyFlags.HostCoherent;
        ///<summary>Memory will be cached by the host</summary>
        public const VkMemoryPropertyFlags VK_MEMORY_PROPERTY_HOST_CACHED_BIT = VkMemoryPropertyFlags.HostCached;
        ///<summary>Memory may be allocated by the driver when it is required</summary>
        public const VkMemoryPropertyFlags VK_MEMORY_PROPERTY_LAZILY_ALLOCATED_BIT = VkMemoryPropertyFlags.LazilyAllocated;
    }

    [Flags]
    public enum VkMemoryHeapFlags
    {
        None = 0,
        ///<summary>If set, heap represents device memory</summary>
        DeviceLocal = 1,
    }
    public static partial class RawConstants
    {
        ///<summary>If set, heap represents device memory</summary>
        public const VkMemoryHeapFlags VK_MEMORY_HEAP_DEVICE_LOCAL_BIT = VkMemoryHeapFlags.DeviceLocal;
    }

    [Flags]
    public enum VkAccessFlags
    {
        None = 0,
        ///<summary>Controls coherency of indirect command reads</summary>
        IndirectCommandRead = 1,
        ///<summary>Controls coherency of index reads</summary>
        IndexRead = 2,
        ///<summary>Controls coherency of vertex attribute reads</summary>
        VertexAttributeRead = 4,
        ///<summary>Controls coherency of uniform buffer reads</summary>
        UniformRead = 8,
        ///<summary>Controls coherency of input attachment reads</summary>
        InputAttachmentRead = 16,
        ///<summary>Controls coherency of shader reads</summary>
        ShaderRead = 32,
        ///<summary>Controls coherency of shader writes</summary>
        ShaderWrite = 64,
        ///<summary>Controls coherency of color attachment reads</summary>
        ColorAttachmentRead = 128,
        ///<summary>Controls coherency of color attachment writes</summary>
        ColorAttachmentWrite = 256,
        ///<summary>Controls coherency of depth/stencil attachment reads</summary>
        DepthStencilAttachmentRead = 512,
        ///<summary>Controls coherency of depth/stencil attachment writes</summary>
        DepthStencilAttachmentWrite = 1024,
        ///<summary>Controls coherency of transfer reads</summary>
        TransferRead = 2048,
        ///<summary>Controls coherency of transfer writes</summary>
        TransferWrite = 4096,
        ///<summary>Controls coherency of host reads</summary>
        HostRead = 8192,
        ///<summary>Controls coherency of host writes</summary>
        HostWrite = 16384,
        ///<summary>Controls coherency of memory reads</summary>
        MemoryRead = 32768,
        ///<summary>Controls coherency of memory writes</summary>
        MemoryWrite = 65536,
        CommandProcessRead = 131072,
        CommandProcessWrite = 262144,
    }
    public static partial class RawConstants
    {
        ///<summary>Controls coherency of indirect command reads</summary>
        public const VkAccessFlags VK_ACCESS_INDIRECT_COMMAND_READ_BIT = VkAccessFlags.IndirectCommandRead;
        ///<summary>Controls coherency of index reads</summary>
        public const VkAccessFlags VK_ACCESS_INDEX_READ_BIT = VkAccessFlags.IndexRead;
        ///<summary>Controls coherency of vertex attribute reads</summary>
        public const VkAccessFlags VK_ACCESS_VERTEX_ATTRIBUTE_READ_BIT = VkAccessFlags.VertexAttributeRead;
        ///<summary>Controls coherency of uniform buffer reads</summary>
        public const VkAccessFlags VK_ACCESS_UNIFORM_READ_BIT = VkAccessFlags.UniformRead;
        ///<summary>Controls coherency of input attachment reads</summary>
        public const VkAccessFlags VK_ACCESS_INPUT_ATTACHMENT_READ_BIT = VkAccessFlags.InputAttachmentRead;
        ///<summary>Controls coherency of shader reads</summary>
        public const VkAccessFlags VK_ACCESS_SHADER_READ_BIT = VkAccessFlags.ShaderRead;
        ///<summary>Controls coherency of shader writes</summary>
        public const VkAccessFlags VK_ACCESS_SHADER_WRITE_BIT = VkAccessFlags.ShaderWrite;
        ///<summary>Controls coherency of color attachment reads</summary>
        public const VkAccessFlags VK_ACCESS_COLOR_ATTACHMENT_READ_BIT = VkAccessFlags.ColorAttachmentRead;
        ///<summary>Controls coherency of color attachment writes</summary>
        public const VkAccessFlags VK_ACCESS_COLOR_ATTACHMENT_WRITE_BIT = VkAccessFlags.ColorAttachmentWrite;
        ///<summary>Controls coherency of depth/stencil attachment reads</summary>
        public const VkAccessFlags VK_ACCESS_DEPTH_STENCIL_ATTACHMENT_READ_BIT = VkAccessFlags.DepthStencilAttachmentRead;
        ///<summary>Controls coherency of depth/stencil attachment writes</summary>
        public const VkAccessFlags VK_ACCESS_DEPTH_STENCIL_ATTACHMENT_WRITE_BIT = VkAccessFlags.DepthStencilAttachmentWrite;
        ///<summary>Controls coherency of transfer reads</summary>
        public const VkAccessFlags VK_ACCESS_TRANSFER_READ_BIT = VkAccessFlags.TransferRead;
        ///<summary>Controls coherency of transfer writes</summary>
        public const VkAccessFlags VK_ACCESS_TRANSFER_WRITE_BIT = VkAccessFlags.TransferWrite;
        ///<summary>Controls coherency of host reads</summary>
        public const VkAccessFlags VK_ACCESS_HOST_READ_BIT = VkAccessFlags.HostRead;
        ///<summary>Controls coherency of host writes</summary>
        public const VkAccessFlags VK_ACCESS_HOST_WRITE_BIT = VkAccessFlags.HostWrite;
        ///<summary>Controls coherency of memory reads</summary>
        public const VkAccessFlags VK_ACCESS_MEMORY_READ_BIT = VkAccessFlags.MemoryRead;
        ///<summary>Controls coherency of memory writes</summary>
        public const VkAccessFlags VK_ACCESS_MEMORY_WRITE_BIT = VkAccessFlags.MemoryWrite;
        public const VkAccessFlags VK_ACCESS_COMMAND_PROCESS_READ_BIT_NVX = VkAccessFlags.CommandProcessRead;
        public const VkAccessFlags VK_ACCESS_COMMAND_PROCESS_WRITE_BIT_NVX = VkAccessFlags.CommandProcessWrite;
    }

    [Flags]
    public enum VkBufferUsageFlags
    {
        None = 0,
        ///<summary>Can be used as a source of transfer operations</summary>
        TransferSrc = 1,
        ///<summary>Can be used as a destination of transfer operations</summary>
        TransferDst = 2,
        ///<summary>Can be used as TBO</summary>
        UniformTexelBuffer = 4,
        ///<summary>Can be used as IBO</summary>
        StorageTexelBuffer = 8,
        ///<summary>Can be used as UBO</summary>
        UniformBuffer = 16,
        ///<summary>Can be used as SSBO</summary>
        StorageBuffer = 32,
        ///<summary>Can be used as source of fixed-function index fetch (index buffer)</summary>
        IndexBuffer = 64,
        ///<summary>Can be used as source of fixed-function vertex fetch (VBO)</summary>
        VertexBuffer = 128,
        ///<summary>Can be the source of indirect parameters (e.g. indirect buffer, parameter buffer)</summary>
        IndirectBuffer = 256,
    }
    public static partial class RawConstants
    {
        ///<summary>Can be used as a source of transfer operations</summary>
        public const VkBufferUsageFlags VK_BUFFER_USAGE_TRANSFER_SRC_BIT = VkBufferUsageFlags.TransferSrc;
        ///<summary>Can be used as a destination of transfer operations</summary>
        public const VkBufferUsageFlags VK_BUFFER_USAGE_TRANSFER_DST_BIT = VkBufferUsageFlags.TransferDst;
        ///<summary>Can be used as TBO</summary>
        public const VkBufferUsageFlags VK_BUFFER_USAGE_UNIFORM_TEXEL_BUFFER_BIT = VkBufferUsageFlags.UniformTexelBuffer;
        ///<summary>Can be used as IBO</summary>
        public const VkBufferUsageFlags VK_BUFFER_USAGE_STORAGE_TEXEL_BUFFER_BIT = VkBufferUsageFlags.StorageTexelBuffer;
        ///<summary>Can be used as UBO</summary>
        public const VkBufferUsageFlags VK_BUFFER_USAGE_UNIFORM_BUFFER_BIT = VkBufferUsageFlags.UniformBuffer;
        ///<summary>Can be used as SSBO</summary>
        public const VkBufferUsageFlags VK_BUFFER_USAGE_STORAGE_BUFFER_BIT = VkBufferUsageFlags.StorageBuffer;
        ///<summary>Can be used as source of fixed-function index fetch (index buffer)</summary>
        public const VkBufferUsageFlags VK_BUFFER_USAGE_INDEX_BUFFER_BIT = VkBufferUsageFlags.IndexBuffer;
        ///<summary>Can be used as source of fixed-function vertex fetch (VBO)</summary>
        public const VkBufferUsageFlags VK_BUFFER_USAGE_VERTEX_BUFFER_BIT = VkBufferUsageFlags.VertexBuffer;
        ///<summary>Can be the source of indirect parameters (e.g. indirect buffer, parameter buffer)</summary>
        public const VkBufferUsageFlags VK_BUFFER_USAGE_INDIRECT_BUFFER_BIT = VkBufferUsageFlags.IndirectBuffer;
    }

    [Flags]
    public enum VkBufferCreateFlags
    {
        None = 0,
        ///<summary>Buffer should support sparse backing</summary>
        SparseBinding = 1,
        ///<summary>Buffer should support sparse backing with partial residency</summary>
        SparseResidency = 2,
        ///<summary>Buffer should support constent data access to physical memory ranges mapped into multiple locations of sparse buffers</summary>
        SparseAliased = 4,
    }
    public static partial class RawConstants
    {
        ///<summary>Buffer should support sparse backing</summary>
        public const VkBufferCreateFlags VK_BUFFER_CREATE_SPARSE_BINDING_BIT = VkBufferCreateFlags.SparseBinding;
        ///<summary>Buffer should support sparse backing with partial residency</summary>
        public const VkBufferCreateFlags VK_BUFFER_CREATE_SPARSE_RESIDENCY_BIT = VkBufferCreateFlags.SparseResidency;
        ///<summary>Buffer should support constent data access to physical memory ranges mapped into multiple locations of sparse buffers</summary>
        public const VkBufferCreateFlags VK_BUFFER_CREATE_SPARSE_ALIASED_BIT = VkBufferCreateFlags.SparseAliased;
    }

    [Flags]
    public enum VkShaderStageFlags
    {
        None = 0,
        Vertex = 1,
        TessellationControl = 2,
        TessellationEvaluation = 4,
        Geometry = 8,
        Fragment = 16,
        Compute = 32,
        AllGraphics = 31,
        All = 2147483647,
    }
    public static partial class RawConstants
    {
        public const VkShaderStageFlags VK_SHADER_STAGE_VERTEX_BIT = VkShaderStageFlags.Vertex;
        public const VkShaderStageFlags VK_SHADER_STAGE_TESSELLATION_CONTROL_BIT = VkShaderStageFlags.TessellationControl;
        public const VkShaderStageFlags VK_SHADER_STAGE_TESSELLATION_EVALUATION_BIT = VkShaderStageFlags.TessellationEvaluation;
        public const VkShaderStageFlags VK_SHADER_STAGE_GEOMETRY_BIT = VkShaderStageFlags.Geometry;
        public const VkShaderStageFlags VK_SHADER_STAGE_FRAGMENT_BIT = VkShaderStageFlags.Fragment;
        public const VkShaderStageFlags VK_SHADER_STAGE_COMPUTE_BIT = VkShaderStageFlags.Compute;
        public const VkShaderStageFlags VK_SHADER_STAGE_ALL_GRAPHICS = VkShaderStageFlags.AllGraphics;
        public const VkShaderStageFlags VK_SHADER_STAGE_ALL = VkShaderStageFlags.All;
    }

    [Flags]
    public enum VkImageUsageFlags
    {
        None = 0,
        ///<summary>Can be used as a source of transfer operations</summary>
        TransferSrc = 1,
        ///<summary>Can be used as a destination of transfer operations</summary>
        TransferDst = 2,
        ///<summary>Can be sampled from (SAMPLED_IMAGE and COMBINED_IMAGE_SAMPLER descriptor types)</summary>
        Sampled = 4,
        ///<summary>Can be used as storage image (STORAGE_IMAGE descriptor type)</summary>
        Storage = 8,
        ///<summary>Can be used as framebuffer color attachment</summary>
        ColorAttachment = 16,
        ///<summary>Can be used as framebuffer depth/stencil attachment</summary>
        DepthStencilAttachment = 32,
        ///<summary>Image data not needed outside of rendering</summary>
        TransientAttachment = 64,
        ///<summary>Can be used as framebuffer input attachment</summary>
        InputAttachment = 128,
    }
    public static partial class RawConstants
    {
        ///<summary>Can be used as a source of transfer operations</summary>
        public const VkImageUsageFlags VK_IMAGE_USAGE_TRANSFER_SRC_BIT = VkImageUsageFlags.TransferSrc;
        ///<summary>Can be used as a destination of transfer operations</summary>
        public const VkImageUsageFlags VK_IMAGE_USAGE_TRANSFER_DST_BIT = VkImageUsageFlags.TransferDst;
        ///<summary>Can be sampled from (SAMPLED_IMAGE and COMBINED_IMAGE_SAMPLER descriptor types)</summary>
        public const VkImageUsageFlags VK_IMAGE_USAGE_SAMPLED_BIT = VkImageUsageFlags.Sampled;
        ///<summary>Can be used as storage image (STORAGE_IMAGE descriptor type)</summary>
        public const VkImageUsageFlags VK_IMAGE_USAGE_STORAGE_BIT = VkImageUsageFlags.Storage;
        ///<summary>Can be used as framebuffer color attachment</summary>
        public const VkImageUsageFlags VK_IMAGE_USAGE_COLOR_ATTACHMENT_BIT = VkImageUsageFlags.ColorAttachment;
        ///<summary>Can be used as framebuffer depth/stencil attachment</summary>
        public const VkImageUsageFlags VK_IMAGE_USAGE_DEPTH_STENCIL_ATTACHMENT_BIT = VkImageUsageFlags.DepthStencilAttachment;
        ///<summary>Image data not needed outside of rendering</summary>
        public const VkImageUsageFlags VK_IMAGE_USAGE_TRANSIENT_ATTACHMENT_BIT = VkImageUsageFlags.TransientAttachment;
        ///<summary>Can be used as framebuffer input attachment</summary>
        public const VkImageUsageFlags VK_IMAGE_USAGE_INPUT_ATTACHMENT_BIT = VkImageUsageFlags.InputAttachment;
    }

    [Flags]
    public enum VkImageCreateFlags
    {
        None = 0,
        ///<summary>Image should support sparse backing</summary>
        SparseBinding = 1,
        ///<summary>Image should support sparse backing with partial residency</summary>
        SparseResidency = 2,
        ///<summary>Image should support constent data access to physical memory ranges mapped into multiple locations of sparse images</summary>
        SparseAliased = 4,
        ///<summary>Allows image views to have different format than the base image</summary>
        MutableFormat = 8,
        ///<summary>Allows creating image views with cube type from the created image</summary>
        CubeCompatible = 16,
    }
    public static partial class RawConstants
    {
        ///<summary>Image should support sparse backing</summary>
        public const VkImageCreateFlags VK_IMAGE_CREATE_SPARSE_BINDING_BIT = VkImageCreateFlags.SparseBinding;
        ///<summary>Image should support sparse backing with partial residency</summary>
        public const VkImageCreateFlags VK_IMAGE_CREATE_SPARSE_RESIDENCY_BIT = VkImageCreateFlags.SparseResidency;
        ///<summary>Image should support constent data access to physical memory ranges mapped into multiple locations of sparse images</summary>
        public const VkImageCreateFlags VK_IMAGE_CREATE_SPARSE_ALIASED_BIT = VkImageCreateFlags.SparseAliased;
        ///<summary>Allows image views to have different format than the base image</summary>
        public const VkImageCreateFlags VK_IMAGE_CREATE_MUTABLE_FORMAT_BIT = VkImageCreateFlags.MutableFormat;
        ///<summary>Allows creating image views with cube type from the created image</summary>
        public const VkImageCreateFlags VK_IMAGE_CREATE_CUBE_COMPATIBLE_BIT = VkImageCreateFlags.CubeCompatible;
    }

    [Flags]
    public enum VkPipelineCreateFlags
    {
        None = 0,
        DisableOptimization = 1,
        AllowDerivatives = 2,
        Derivative = 4,
    }
    public static partial class RawConstants
    {
        public const VkPipelineCreateFlags VK_PIPELINE_CREATE_DISABLE_OPTIMIZATION_BIT = VkPipelineCreateFlags.DisableOptimization;
        public const VkPipelineCreateFlags VK_PIPELINE_CREATE_ALLOW_DERIVATIVES_BIT = VkPipelineCreateFlags.AllowDerivatives;
        public const VkPipelineCreateFlags VK_PIPELINE_CREATE_DERIVATIVE_BIT = VkPipelineCreateFlags.Derivative;
    }

    [Flags]
    public enum VkColorComponentFlags
    {
        None = 0,
        R = 1,
        G = 2,
        B = 4,
        A = 8,
    }
    public static partial class RawConstants
    {
        public const VkColorComponentFlags VK_COLOR_COMPONENT_R_BIT = VkColorComponentFlags.R;
        public const VkColorComponentFlags VK_COLOR_COMPONENT_G_BIT = VkColorComponentFlags.G;
        public const VkColorComponentFlags VK_COLOR_COMPONENT_B_BIT = VkColorComponentFlags.B;
        public const VkColorComponentFlags VK_COLOR_COMPONENT_A_BIT = VkColorComponentFlags.A;
    }

    [Flags]
    public enum VkFenceCreateFlags
    {
        None = 0,
        Signaled = 1,
    }
    public static partial class RawConstants
    {
        public const VkFenceCreateFlags VK_FENCE_CREATE_SIGNALED_BIT = VkFenceCreateFlags.Signaled;
    }

    [Flags]
    public enum VkFormatFeatureFlags
    {
        None = 0,
        ///<summary>Format can be used for sampled images (SAMPLED_IMAGE and COMBINED_IMAGE_SAMPLER descriptor types)</summary>
        SampledImage = 1,
        ///<summary>Format can be used for storage images (STORAGE_IMAGE descriptor type)</summary>
        StorageImage = 2,
        ///<summary>Format supports atomic operations in case it is used for storage images</summary>
        StorageImageAtomic = 4,
        ///<summary>Format can be used for uniform texel buffers (TBOs)</summary>
        UniformTexelBuffer = 8,
        ///<summary>Format can be used for storage texel buffers (IBOs)</summary>
        StorageTexelBuffer = 16,
        ///<summary>Format supports atomic operations in case it is used for storage texel buffers</summary>
        StorageTexelBufferAtomic = 32,
        ///<summary>Format can be used for vertex buffers (VBOs)</summary>
        VertexBuffer = 64,
        ///<summary>Format can be used for color attachment images</summary>
        ColorAttachment = 128,
        ///<summary>Format supports blending in case it is used for color attachment images</summary>
        ColorAttachmentBlend = 256,
        ///<summary>Format can be used for depth/stencil attachment images</summary>
        DepthStencilAttachment = 512,
        ///<summary>Format can be used as the source image of blits with vkCmdBlitImage</summary>
        BlitSrc = 1024,
        ///<summary>Format can be used as the destination image of blits with vkCmdBlitImage</summary>
        BlitDst = 2048,
        ///<summary>Format can be filtered with VK_FILTER_LINEAR when being sampled</summary>
        SampledImageFilterLinear = 4096,
        SampledImageFilterCubicImg = 8192,
    }
    public static partial class RawConstants
    {
        ///<summary>Format can be used for sampled images (SAMPLED_IMAGE and COMBINED_IMAGE_SAMPLER descriptor types)</summary>
        public const VkFormatFeatureFlags VK_FORMAT_FEATURE_SAMPLED_IMAGE_BIT = VkFormatFeatureFlags.SampledImage;
        ///<summary>Format can be used for storage images (STORAGE_IMAGE descriptor type)</summary>
        public const VkFormatFeatureFlags VK_FORMAT_FEATURE_STORAGE_IMAGE_BIT = VkFormatFeatureFlags.StorageImage;
        ///<summary>Format supports atomic operations in case it is used for storage images</summary>
        public const VkFormatFeatureFlags VK_FORMAT_FEATURE_STORAGE_IMAGE_ATOMIC_BIT = VkFormatFeatureFlags.StorageImageAtomic;
        ///<summary>Format can be used for uniform texel buffers (TBOs)</summary>
        public const VkFormatFeatureFlags VK_FORMAT_FEATURE_UNIFORM_TEXEL_BUFFER_BIT = VkFormatFeatureFlags.UniformTexelBuffer;
        ///<summary>Format can be used for storage texel buffers (IBOs)</summary>
        public const VkFormatFeatureFlags VK_FORMAT_FEATURE_STORAGE_TEXEL_BUFFER_BIT = VkFormatFeatureFlags.StorageTexelBuffer;
        ///<summary>Format supports atomic operations in case it is used for storage texel buffers</summary>
        public const VkFormatFeatureFlags VK_FORMAT_FEATURE_STORAGE_TEXEL_BUFFER_ATOMIC_BIT = VkFormatFeatureFlags.StorageTexelBufferAtomic;
        ///<summary>Format can be used for vertex buffers (VBOs)</summary>
        public const VkFormatFeatureFlags VK_FORMAT_FEATURE_VERTEX_BUFFER_BIT = VkFormatFeatureFlags.VertexBuffer;
        ///<summary>Format can be used for color attachment images</summary>
        public const VkFormatFeatureFlags VK_FORMAT_FEATURE_COLOR_ATTACHMENT_BIT = VkFormatFeatureFlags.ColorAttachment;
        ///<summary>Format supports blending in case it is used for color attachment images</summary>
        public const VkFormatFeatureFlags VK_FORMAT_FEATURE_COLOR_ATTACHMENT_BLEND_BIT = VkFormatFeatureFlags.ColorAttachmentBlend;
        ///<summary>Format can be used for depth/stencil attachment images</summary>
        public const VkFormatFeatureFlags VK_FORMAT_FEATURE_DEPTH_STENCIL_ATTACHMENT_BIT = VkFormatFeatureFlags.DepthStencilAttachment;
        ///<summary>Format can be used as the source image of blits with vkCmdBlitImage</summary>
        public const VkFormatFeatureFlags VK_FORMAT_FEATURE_BLIT_SRC_BIT = VkFormatFeatureFlags.BlitSrc;
        ///<summary>Format can be used as the destination image of blits with vkCmdBlitImage</summary>
        public const VkFormatFeatureFlags VK_FORMAT_FEATURE_BLIT_DST_BIT = VkFormatFeatureFlags.BlitDst;
        ///<summary>Format can be filtered with VK_FILTER_LINEAR when being sampled</summary>
        public const VkFormatFeatureFlags VK_FORMAT_FEATURE_SAMPLED_IMAGE_FILTER_LINEAR_BIT = VkFormatFeatureFlags.SampledImageFilterLinear;
        public const VkFormatFeatureFlags VK_FORMAT_FEATURE_SAMPLED_IMAGE_FILTER_CUBIC_BIT_IMG = VkFormatFeatureFlags.SampledImageFilterCubicImg;
    }

    [Flags]
    public enum VkQueryControlFlags
    {
        None = 0,
        ///<summary>Require precise results to be collected by the query</summary>
        Precise = 1,
    }
    public static partial class RawConstants
    {
        ///<summary>Require precise results to be collected by the query</summary>
        public const VkQueryControlFlags VK_QUERY_CONTROL_PRECISE_BIT = VkQueryControlFlags.Precise;
    }

    [Flags]
    public enum VkQueryResultFlags
    {
        None = 0,
        ///<summary>Results of the queries are written to the destination buffer as 64-bit values</summary>
        _64 = 1,
        ///<summary>Results of the queries are waited on before proceeding with the result copy</summary>
        Wait = 2,
        ///<summary>Besides the results of the query, the availability of the results is also written</summary>
        WithAvailability = 4,
        ///<summary>Copy the partial results of the query even if the final results are not available</summary>
        Partial = 8,
    }
    public static partial class RawConstants
    {
        ///<summary>Results of the queries are written to the destination buffer as 64-bit values</summary>
        public const VkQueryResultFlags VK_QUERY_RESULT_64_BIT = VkQueryResultFlags._64;
        ///<summary>Results of the queries are waited on before proceeding with the result copy</summary>
        public const VkQueryResultFlags VK_QUERY_RESULT_WAIT_BIT = VkQueryResultFlags.Wait;
        ///<summary>Besides the results of the query, the availability of the results is also written</summary>
        public const VkQueryResultFlags VK_QUERY_RESULT_WITH_AVAILABILITY_BIT = VkQueryResultFlags.WithAvailability;
        ///<summary>Copy the partial results of the query even if the final results are not available</summary>
        public const VkQueryResultFlags VK_QUERY_RESULT_PARTIAL_BIT = VkQueryResultFlags.Partial;
    }

    [Flags]
    public enum VkCommandBufferUsageFlags
    {
        None = 0,
        OneTimeSubmit = 1,
        RenderPassContinue = 2,
        ///<summary>Command buffer may be submitted/executed more than once simultaneously</summary>
        SimultaneousUse = 4,
    }
    public static partial class RawConstants
    {
        public const VkCommandBufferUsageFlags VK_COMMAND_BUFFER_USAGE_ONE_TIME_SUBMIT_BIT = VkCommandBufferUsageFlags.OneTimeSubmit;
        public const VkCommandBufferUsageFlags VK_COMMAND_BUFFER_USAGE_RENDER_PASS_CONTINUE_BIT = VkCommandBufferUsageFlags.RenderPassContinue;
        ///<summary>Command buffer may be submitted/executed more than once simultaneously</summary>
        public const VkCommandBufferUsageFlags VK_COMMAND_BUFFER_USAGE_SIMULTANEOUS_USE_BIT = VkCommandBufferUsageFlags.SimultaneousUse;
    }

    [Flags]
    public enum VkQueryPipelineStatisticFlags
    {
        None = 0,
        ///<summary>Optional</summary>
        InputAssemblyVertices = 1,
        ///<summary>Optional</summary>
        InputAssemblyPrimitives = 2,
        ///<summary>Optional</summary>
        VertexShaderInvocations = 4,
        ///<summary>Optional</summary>
        GeometryShaderInvocations = 8,
        ///<summary>Optional</summary>
        GeometryShaderPrimitives = 16,
        ///<summary>Optional</summary>
        ClippingInvocations = 32,
        ///<summary>Optional</summary>
        ClippingPrimitives = 64,
        ///<summary>Optional</summary>
        FragmentShaderInvocations = 128,
        ///<summary>Optional</summary>
        TessellationControlShaderPatches = 256,
        ///<summary>Optional</summary>
        TessellationEvaluationShaderInvocations = 512,
        ///<summary>Optional</summary>
        ComputeShaderInvocations = 1024,
    }
    public static partial class RawConstants
    {
        ///<summary>Optional</summary>
        public const VkQueryPipelineStatisticFlags VK_QUERY_PIPELINE_STATISTIC_INPUT_ASSEMBLY_VERTICES_BIT = VkQueryPipelineStatisticFlags.InputAssemblyVertices;
        ///<summary>Optional</summary>
        public const VkQueryPipelineStatisticFlags VK_QUERY_PIPELINE_STATISTIC_INPUT_ASSEMBLY_PRIMITIVES_BIT = VkQueryPipelineStatisticFlags.InputAssemblyPrimitives;
        ///<summary>Optional</summary>
        public const VkQueryPipelineStatisticFlags VK_QUERY_PIPELINE_STATISTIC_VERTEX_SHADER_INVOCATIONS_BIT = VkQueryPipelineStatisticFlags.VertexShaderInvocations;
        ///<summary>Optional</summary>
        public const VkQueryPipelineStatisticFlags VK_QUERY_PIPELINE_STATISTIC_GEOMETRY_SHADER_INVOCATIONS_BIT = VkQueryPipelineStatisticFlags.GeometryShaderInvocations;
        ///<summary>Optional</summary>
        public const VkQueryPipelineStatisticFlags VK_QUERY_PIPELINE_STATISTIC_GEOMETRY_SHADER_PRIMITIVES_BIT = VkQueryPipelineStatisticFlags.GeometryShaderPrimitives;
        ///<summary>Optional</summary>
        public const VkQueryPipelineStatisticFlags VK_QUERY_PIPELINE_STATISTIC_CLIPPING_INVOCATIONS_BIT = VkQueryPipelineStatisticFlags.ClippingInvocations;
        ///<summary>Optional</summary>
        public const VkQueryPipelineStatisticFlags VK_QUERY_PIPELINE_STATISTIC_CLIPPING_PRIMITIVES_BIT = VkQueryPipelineStatisticFlags.ClippingPrimitives;
        ///<summary>Optional</summary>
        public const VkQueryPipelineStatisticFlags VK_QUERY_PIPELINE_STATISTIC_FRAGMENT_SHADER_INVOCATIONS_BIT = VkQueryPipelineStatisticFlags.FragmentShaderInvocations;
        ///<summary>Optional</summary>
        public const VkQueryPipelineStatisticFlags VK_QUERY_PIPELINE_STATISTIC_TESSELLATION_CONTROL_SHADER_PATCHES_BIT = VkQueryPipelineStatisticFlags.TessellationControlShaderPatches;
        ///<summary>Optional</summary>
        public const VkQueryPipelineStatisticFlags VK_QUERY_PIPELINE_STATISTIC_TESSELLATION_EVALUATION_SHADER_INVOCATIONS_BIT = VkQueryPipelineStatisticFlags.TessellationEvaluationShaderInvocations;
        ///<summary>Optional</summary>
        public const VkQueryPipelineStatisticFlags VK_QUERY_PIPELINE_STATISTIC_COMPUTE_SHADER_INVOCATIONS_BIT = VkQueryPipelineStatisticFlags.ComputeShaderInvocations;
    }

    [Flags]
    public enum VkImageAspectFlags
    {
        None = 0,
        Color = 1,
        Depth = 2,
        Stencil = 4,
        Metadata = 8,
    }
    public static partial class RawConstants
    {
        public const VkImageAspectFlags VK_IMAGE_ASPECT_COLOR_BIT = VkImageAspectFlags.Color;
        public const VkImageAspectFlags VK_IMAGE_ASPECT_DEPTH_BIT = VkImageAspectFlags.Depth;
        public const VkImageAspectFlags VK_IMAGE_ASPECT_STENCIL_BIT = VkImageAspectFlags.Stencil;
        public const VkImageAspectFlags VK_IMAGE_ASPECT_METADATA_BIT = VkImageAspectFlags.Metadata;
    }

    [Flags]
    public enum VkSparseImageFormatFlags
    {
        None = 0,
        ///<summary>Image uses a single mip tail region for all array layers</summary>
        SingleMiptail = 1,
        ///<summary>Image requires mip level dimensions to be an integer multiple of the sparse image block dimensions for non-tail mip levels.</summary>
        AlignedMipSize = 2,
        ///<summary>Image uses a non-standard sparse image block dimensions</summary>
        NonstandardBlockSize = 4,
    }
    public static partial class RawConstants
    {
        ///<summary>Image uses a single mip tail region for all array layers</summary>
        public const VkSparseImageFormatFlags VK_SPARSE_IMAGE_FORMAT_SINGLE_MIPTAIL_BIT = VkSparseImageFormatFlags.SingleMiptail;
        ///<summary>Image requires mip level dimensions to be an integer multiple of the sparse image block dimensions for non-tail mip levels.</summary>
        public const VkSparseImageFormatFlags VK_SPARSE_IMAGE_FORMAT_ALIGNED_MIP_SIZE_BIT = VkSparseImageFormatFlags.AlignedMipSize;
        ///<summary>Image uses a non-standard sparse image block dimensions</summary>
        public const VkSparseImageFormatFlags VK_SPARSE_IMAGE_FORMAT_NONSTANDARD_BLOCK_SIZE_BIT = VkSparseImageFormatFlags.NonstandardBlockSize;
    }

    [Flags]
    public enum VkSparseMemoryBindFlags
    {
        None = 0,
        ///<summary>Operation binds resource metadata to memory</summary>
        Metadata = 1,
    }
    public static partial class RawConstants
    {
        ///<summary>Operation binds resource metadata to memory</summary>
        public const VkSparseMemoryBindFlags VK_SPARSE_MEMORY_BIND_METADATA_BIT = VkSparseMemoryBindFlags.Metadata;
    }

    [Flags]
    public enum VkPipelineStageFlags
    {
        None = 0,
        ///<summary>Before subsequent commands are processed</summary>
        TopOfPipe = 1,
        ///<summary>Draw/DispatchIndirect command fetch</summary>
        DrawIndirect = 2,
        ///<summary>Vertex/index fetch</summary>
        VertexInput = 4,
        ///<summary>Vertex shading</summary>
        VertexShader = 8,
        ///<summary>Tessellation control shading</summary>
        TessellationControlShader = 16,
        ///<summary>Tessellation evaluation shading</summary>
        TessellationEvaluationShader = 32,
        ///<summary>Geometry shading</summary>
        GeometryShader = 64,
        ///<summary>Fragment shading</summary>
        FragmentShader = 128,
        ///<summary>Early fragment (depth and stencil) tests</summary>
        EarlyFragmentTests = 256,
        ///<summary>Late fragment (depth and stencil) tests</summary>
        LateFragmentTests = 512,
        ///<summary>Color attachment writes</summary>
        ColorAttachmentOutput = 1024,
        ///<summary>Compute shading</summary>
        ComputeShader = 2048,
        ///<summary>Transfer/copy operations</summary>
        Transfer = 4096,
        ///<summary>After previous commands have completed</summary>
        BottomOfPipe = 8192,
        ///<summary>Indicates host (CPU) is a source/sink of the dependency</summary>
        Host = 16384,
        ///<summary>All stages of the graphics pipeline</summary>
        AllGraphics = 32768,
        ///<summary>All stages supported on the queue</summary>
        AllCommands = 65536,
        CommandProcess = 131072,
    }
    public static partial class RawConstants
    {
        ///<summary>Before subsequent commands are processed</summary>
        public const VkPipelineStageFlags VK_PIPELINE_STAGE_TOP_OF_PIPE_BIT = VkPipelineStageFlags.TopOfPipe;
        ///<summary>Draw/DispatchIndirect command fetch</summary>
        public const VkPipelineStageFlags VK_PIPELINE_STAGE_DRAW_INDIRECT_BIT = VkPipelineStageFlags.DrawIndirect;
        ///<summary>Vertex/index fetch</summary>
        public const VkPipelineStageFlags VK_PIPELINE_STAGE_VERTEX_INPUT_BIT = VkPipelineStageFlags.VertexInput;
        ///<summary>Vertex shading</summary>
        public const VkPipelineStageFlags VK_PIPELINE_STAGE_VERTEX_SHADER_BIT = VkPipelineStageFlags.VertexShader;
        ///<summary>Tessellation control shading</summary>
        public const VkPipelineStageFlags VK_PIPELINE_STAGE_TESSELLATION_CONTROL_SHADER_BIT = VkPipelineStageFlags.TessellationControlShader;
        ///<summary>Tessellation evaluation shading</summary>
        public const VkPipelineStageFlags VK_PIPELINE_STAGE_TESSELLATION_EVALUATION_SHADER_BIT = VkPipelineStageFlags.TessellationEvaluationShader;
        ///<summary>Geometry shading</summary>
        public const VkPipelineStageFlags VK_PIPELINE_STAGE_GEOMETRY_SHADER_BIT = VkPipelineStageFlags.GeometryShader;
        ///<summary>Fragment shading</summary>
        public const VkPipelineStageFlags VK_PIPELINE_STAGE_FRAGMENT_SHADER_BIT = VkPipelineStageFlags.FragmentShader;
        ///<summary>Early fragment (depth and stencil) tests</summary>
        public const VkPipelineStageFlags VK_PIPELINE_STAGE_EARLY_FRAGMENT_TESTS_BIT = VkPipelineStageFlags.EarlyFragmentTests;
        ///<summary>Late fragment (depth and stencil) tests</summary>
        public const VkPipelineStageFlags VK_PIPELINE_STAGE_LATE_FRAGMENT_TESTS_BIT = VkPipelineStageFlags.LateFragmentTests;
        ///<summary>Color attachment writes</summary>
        public const VkPipelineStageFlags VK_PIPELINE_STAGE_COLOR_ATTACHMENT_OUTPUT_BIT = VkPipelineStageFlags.ColorAttachmentOutput;
        ///<summary>Compute shading</summary>
        public const VkPipelineStageFlags VK_PIPELINE_STAGE_COMPUTE_SHADER_BIT = VkPipelineStageFlags.ComputeShader;
        ///<summary>Transfer/copy operations</summary>
        public const VkPipelineStageFlags VK_PIPELINE_STAGE_TRANSFER_BIT = VkPipelineStageFlags.Transfer;
        ///<summary>After previous commands have completed</summary>
        public const VkPipelineStageFlags VK_PIPELINE_STAGE_BOTTOM_OF_PIPE_BIT = VkPipelineStageFlags.BottomOfPipe;
        ///<summary>Indicates host (CPU) is a source/sink of the dependency</summary>
        public const VkPipelineStageFlags VK_PIPELINE_STAGE_HOST_BIT = VkPipelineStageFlags.Host;
        ///<summary>All stages of the graphics pipeline</summary>
        public const VkPipelineStageFlags VK_PIPELINE_STAGE_ALL_GRAPHICS_BIT = VkPipelineStageFlags.AllGraphics;
        ///<summary>All stages supported on the queue</summary>
        public const VkPipelineStageFlags VK_PIPELINE_STAGE_ALL_COMMANDS_BIT = VkPipelineStageFlags.AllCommands;
        public const VkPipelineStageFlags VK_PIPELINE_STAGE_COMMAND_PROCESS_BIT_NVX = VkPipelineStageFlags.CommandProcess;
    }

    [Flags]
    public enum VkCommandPoolCreateFlags
    {
        None = 0,
        ///<summary>Command buffers have a short lifetime</summary>
        Transient = 1,
        ///<summary>Command buffers may release their memory individually</summary>
        ResetCommandBuffer = 2,
    }
    public static partial class RawConstants
    {
        ///<summary>Command buffers have a short lifetime</summary>
        public const VkCommandPoolCreateFlags VK_COMMAND_POOL_CREATE_TRANSIENT_BIT = VkCommandPoolCreateFlags.Transient;
        ///<summary>Command buffers may release their memory individually</summary>
        public const VkCommandPoolCreateFlags VK_COMMAND_POOL_CREATE_RESET_COMMAND_BUFFER_BIT = VkCommandPoolCreateFlags.ResetCommandBuffer;
    }

    [Flags]
    public enum VkCommandPoolResetFlags
    {
        None = 0,
        ///<summary>Release resources owned by the pool</summary>
        ReleaseResources = 1,
    }
    public static partial class RawConstants
    {
        ///<summary>Release resources owned by the pool</summary>
        public const VkCommandPoolResetFlags VK_COMMAND_POOL_RESET_RELEASE_RESOURCES_BIT = VkCommandPoolResetFlags.ReleaseResources;
    }

    [Flags]
    public enum VkCommandBufferResetFlags
    {
        None = 0,
        ///<summary>Release resources owned by the buffer</summary>
        ReleaseResources = 1,
    }
    public static partial class RawConstants
    {
        ///<summary>Release resources owned by the buffer</summary>
        public const VkCommandBufferResetFlags VK_COMMAND_BUFFER_RESET_RELEASE_RESOURCES_BIT = VkCommandBufferResetFlags.ReleaseResources;
    }

    [Flags]
    public enum VkSampleCountFlags
    {
        None = 0,
        ///<summary>Sample count 1 supported</summary>
        _1 = 1,
        ///<summary>Sample count 2 supported</summary>
        _2 = 2,
        ///<summary>Sample count 4 supported</summary>
        _4 = 4,
        ///<summary>Sample count 8 supported</summary>
        _8 = 8,
        ///<summary>Sample count 16 supported</summary>
        _16 = 16,
        ///<summary>Sample count 32 supported</summary>
        _32 = 32,
        ///<summary>Sample count 64 supported</summary>
        _64 = 64,
    }
    public static partial class RawConstants
    {
        ///<summary>Sample count 1 supported</summary>
        public const VkSampleCountFlags VK_SAMPLE_COUNT_1_BIT = VkSampleCountFlags._1;
        ///<summary>Sample count 2 supported</summary>
        public const VkSampleCountFlags VK_SAMPLE_COUNT_2_BIT = VkSampleCountFlags._2;
        ///<summary>Sample count 4 supported</summary>
        public const VkSampleCountFlags VK_SAMPLE_COUNT_4_BIT = VkSampleCountFlags._4;
        ///<summary>Sample count 8 supported</summary>
        public const VkSampleCountFlags VK_SAMPLE_COUNT_8_BIT = VkSampleCountFlags._8;
        ///<summary>Sample count 16 supported</summary>
        public const VkSampleCountFlags VK_SAMPLE_COUNT_16_BIT = VkSampleCountFlags._16;
        ///<summary>Sample count 32 supported</summary>
        public const VkSampleCountFlags VK_SAMPLE_COUNT_32_BIT = VkSampleCountFlags._32;
        ///<summary>Sample count 64 supported</summary>
        public const VkSampleCountFlags VK_SAMPLE_COUNT_64_BIT = VkSampleCountFlags._64;
    }

    [Flags]
    public enum VkAttachmentDescriptionFlags
    {
        None = 0,
        ///<summary>The attachment may alias physical memory of another attachment in the same render pass</summary>
        MayAlias = 1,
    }
    public static partial class RawConstants
    {
        ///<summary>The attachment may alias physical memory of another attachment in the same render pass</summary>
        public const VkAttachmentDescriptionFlags VK_ATTACHMENT_DESCRIPTION_MAY_ALIAS_BIT = VkAttachmentDescriptionFlags.MayAlias;
    }

    [Flags]
    public enum VkStencilFaceFlags
    {
        None = 0,
        ///<summary>Front face</summary>
        Front = 1,
        ///<summary>Back face</summary>
        Back = 2,
        ///<summary>Front and back faces</summary>
        FrontAndBack = 3,
    }
    public static partial class RawConstants
    {
        ///<summary>Front face</summary>
        public const VkStencilFaceFlags VK_STENCIL_FACE_FRONT_BIT = VkStencilFaceFlags.Front;
        ///<summary>Back face</summary>
        public const VkStencilFaceFlags VK_STENCIL_FACE_BACK_BIT = VkStencilFaceFlags.Back;
        ///<summary>Front and back faces</summary>
        public const VkStencilFaceFlags VK_STENCIL_FRONT_AND_BACK = VkStencilFaceFlags.FrontAndBack;
    }

    [Flags]
    public enum VkDescriptorPoolCreateFlags
    {
        None = 0,
        ///<summary>Descriptor sets may be freed individually</summary>
        FreeDescriptorSet = 1,
    }
    public static partial class RawConstants
    {
        ///<summary>Descriptor sets may be freed individually</summary>
        public const VkDescriptorPoolCreateFlags VK_DESCRIPTOR_POOL_CREATE_FREE_DESCRIPTOR_SET_BIT = VkDescriptorPoolCreateFlags.FreeDescriptorSet;
    }

    [Flags]
    public enum VkDependencyFlags
    {
        None = 0,
        ///<summary>Dependency is per pixel region </summary>
        ByRegion = 1,
    }
    public static partial class RawConstants
    {
        ///<summary>Dependency is per pixel region </summary>
        public const VkDependencyFlags VK_DEPENDENCY_BY_REGION_BIT = VkDependencyFlags.ByRegion;
    }

    public enum VkPresentModeKHR
    {
        Immediate = 0,
        Mailbox = 1,
        Fifo = 2,
        FifoRelaxed = 3,
    }
    public static partial class RawConstants
    {
        public const VkPresentModeKHR VK_PRESENT_MODE_IMMEDIATE_KHR = VkPresentModeKHR.Immediate;
        public const VkPresentModeKHR VK_PRESENT_MODE_MAILBOX_KHR = VkPresentModeKHR.Mailbox;
        public const VkPresentModeKHR VK_PRESENT_MODE_FIFO_KHR = VkPresentModeKHR.Fifo;
        public const VkPresentModeKHR VK_PRESENT_MODE_FIFO_RELAXED_KHR = VkPresentModeKHR.FifoRelaxed;
    }

    public enum VkColorSpaceKHR
    {
        SrgbNonlinear = 0,
    }
    public static partial class RawConstants
    {
        public const VkColorSpaceKHR VK_COLOR_SPACE_SRGB_NONLINEAR_KHR = VkColorSpaceKHR.SrgbNonlinear;
    }

    [Flags]
    public enum VkDisplayPlaneAlphaFlagsKHR
    {
        None = 0,
        Opaque = 1,
        Global = 2,
        PerPixel = 4,
        PerPixelPremultiplied = 8,
    }
    public static partial class RawConstants
    {
        public const VkDisplayPlaneAlphaFlagsKHR VK_DISPLAY_PLANE_ALPHA_OPAQUE_BIT_KHR = VkDisplayPlaneAlphaFlagsKHR.Opaque;
        public const VkDisplayPlaneAlphaFlagsKHR VK_DISPLAY_PLANE_ALPHA_GLOBAL_BIT_KHR = VkDisplayPlaneAlphaFlagsKHR.Global;
        public const VkDisplayPlaneAlphaFlagsKHR VK_DISPLAY_PLANE_ALPHA_PER_PIXEL_BIT_KHR = VkDisplayPlaneAlphaFlagsKHR.PerPixel;
        public const VkDisplayPlaneAlphaFlagsKHR VK_DISPLAY_PLANE_ALPHA_PER_PIXEL_PREMULTIPLIED_BIT_KHR = VkDisplayPlaneAlphaFlagsKHR.PerPixelPremultiplied;
    }

    [Flags]
    public enum VkCompositeAlphaFlagsKHR
    {
        None = 0,
        Opaque = 1,
        PreMultiplied = 2,
        PostMultiplied = 4,
        Inherit = 8,
    }
    public static partial class RawConstants
    {
        public const VkCompositeAlphaFlagsKHR VK_COMPOSITE_ALPHA_OPAQUE_BIT_KHR = VkCompositeAlphaFlagsKHR.Opaque;
        public const VkCompositeAlphaFlagsKHR VK_COMPOSITE_ALPHA_PRE_MULTIPLIED_BIT_KHR = VkCompositeAlphaFlagsKHR.PreMultiplied;
        public const VkCompositeAlphaFlagsKHR VK_COMPOSITE_ALPHA_POST_MULTIPLIED_BIT_KHR = VkCompositeAlphaFlagsKHR.PostMultiplied;
        public const VkCompositeAlphaFlagsKHR VK_COMPOSITE_ALPHA_INHERIT_BIT_KHR = VkCompositeAlphaFlagsKHR.Inherit;
    }

    [Flags]
    public enum VkSurfaceTransformFlagsKHR
    {
        None = 0,
        Identity = 1,
        Rotate90 = 2,
        Rotate180 = 4,
        Rotate270 = 8,
        HorizontalMirror = 16,
        HorizontalMirrorRotate90 = 32,
        HorizontalMirrorRotate180 = 64,
        HorizontalMirrorRotate270 = 128,
        Inherit = 256,
    }
    public static partial class RawConstants
    {
        public const VkSurfaceTransformFlagsKHR VK_SURFACE_TRANSFORM_IDENTITY_BIT_KHR = VkSurfaceTransformFlagsKHR.Identity;
        public const VkSurfaceTransformFlagsKHR VK_SURFACE_TRANSFORM_ROTATE_90_BIT_KHR = VkSurfaceTransformFlagsKHR.Rotate90;
        public const VkSurfaceTransformFlagsKHR VK_SURFACE_TRANSFORM_ROTATE_180_BIT_KHR = VkSurfaceTransformFlagsKHR.Rotate180;
        public const VkSurfaceTransformFlagsKHR VK_SURFACE_TRANSFORM_ROTATE_270_BIT_KHR = VkSurfaceTransformFlagsKHR.Rotate270;
        public const VkSurfaceTransformFlagsKHR VK_SURFACE_TRANSFORM_HORIZONTAL_MIRROR_BIT_KHR = VkSurfaceTransformFlagsKHR.HorizontalMirror;
        public const VkSurfaceTransformFlagsKHR VK_SURFACE_TRANSFORM_HORIZONTAL_MIRROR_ROTATE_90_BIT_KHR = VkSurfaceTransformFlagsKHR.HorizontalMirrorRotate90;
        public const VkSurfaceTransformFlagsKHR VK_SURFACE_TRANSFORM_HORIZONTAL_MIRROR_ROTATE_180_BIT_KHR = VkSurfaceTransformFlagsKHR.HorizontalMirrorRotate180;
        public const VkSurfaceTransformFlagsKHR VK_SURFACE_TRANSFORM_HORIZONTAL_MIRROR_ROTATE_270_BIT_KHR = VkSurfaceTransformFlagsKHR.HorizontalMirrorRotate270;
        public const VkSurfaceTransformFlagsKHR VK_SURFACE_TRANSFORM_INHERIT_BIT_KHR = VkSurfaceTransformFlagsKHR.Inherit;
    }

    [Flags]
    public enum VkDebugReportFlagsEXT
    {
        None = 0,
        Information = 1,
        Warning = 2,
        PerformanceWarning = 4,
        Error = 8,
        Debug = 16,
    }
    public static partial class RawConstants
    {
        public const VkDebugReportFlagsEXT VK_DEBUG_REPORT_INFORMATION_BIT_EXT = VkDebugReportFlagsEXT.Information;
        public const VkDebugReportFlagsEXT VK_DEBUG_REPORT_WARNING_BIT_EXT = VkDebugReportFlagsEXT.Warning;
        public const VkDebugReportFlagsEXT VK_DEBUG_REPORT_PERFORMANCE_WARNING_BIT_EXT = VkDebugReportFlagsEXT.PerformanceWarning;
        public const VkDebugReportFlagsEXT VK_DEBUG_REPORT_ERROR_BIT_EXT = VkDebugReportFlagsEXT.Error;
        public const VkDebugReportFlagsEXT VK_DEBUG_REPORT_DEBUG_BIT_EXT = VkDebugReportFlagsEXT.Debug;
    }

    public enum VkDebugReportObjectTypeEXT
    {
        Unknown = 0,
        Instance = 1,
        PhysicalDevice = 2,
        Device = 3,
        Queue = 4,
        Semaphore = 5,
        CommandBuffer = 6,
        Fence = 7,
        DeviceMemory = 8,
        Buffer = 9,
        Image = 10,
        Event = 11,
        QueryPool = 12,
        BufferView = 13,
        ImageView = 14,
        ShaderModule = 15,
        PipelineCache = 16,
        PipelineLayout = 17,
        RenderPass = 18,
        Pipeline = 19,
        DescriptorSetLayout = 20,
        Sampler = 21,
        DescriptorPool = 22,
        DescriptorSet = 23,
        Framebuffer = 24,
        CommandPool = 25,
        Surface = 26,
        Swapchain = 27,
        DebugReport = 28,
        Display = 29,
        DisplayMode = 30,
        ObjectTable = 31,
        IndirectCommandsLayout = 32,
    }
    public static partial class RawConstants
    {
        public const VkDebugReportObjectTypeEXT VK_DEBUG_REPORT_OBJECT_TYPE_UNKNOWN_EXT = VkDebugReportObjectTypeEXT.Unknown;
        public const VkDebugReportObjectTypeEXT VK_DEBUG_REPORT_OBJECT_TYPE_INSTANCE_EXT = VkDebugReportObjectTypeEXT.Instance;
        public const VkDebugReportObjectTypeEXT VK_DEBUG_REPORT_OBJECT_TYPE_PHYSICAL_DEVICE_EXT = VkDebugReportObjectTypeEXT.PhysicalDevice;
        public const VkDebugReportObjectTypeEXT VK_DEBUG_REPORT_OBJECT_TYPE_DEVICE_EXT = VkDebugReportObjectTypeEXT.Device;
        public const VkDebugReportObjectTypeEXT VK_DEBUG_REPORT_OBJECT_TYPE_QUEUE_EXT = VkDebugReportObjectTypeEXT.Queue;
        public const VkDebugReportObjectTypeEXT VK_DEBUG_REPORT_OBJECT_TYPE_SEMAPHORE_EXT = VkDebugReportObjectTypeEXT.Semaphore;
        public const VkDebugReportObjectTypeEXT VK_DEBUG_REPORT_OBJECT_TYPE_COMMAND_BUFFER_EXT = VkDebugReportObjectTypeEXT.CommandBuffer;
        public const VkDebugReportObjectTypeEXT VK_DEBUG_REPORT_OBJECT_TYPE_FENCE_EXT = VkDebugReportObjectTypeEXT.Fence;
        public const VkDebugReportObjectTypeEXT VK_DEBUG_REPORT_OBJECT_TYPE_DEVICE_MEMORY_EXT = VkDebugReportObjectTypeEXT.DeviceMemory;
        public const VkDebugReportObjectTypeEXT VK_DEBUG_REPORT_OBJECT_TYPE_BUFFER_EXT = VkDebugReportObjectTypeEXT.Buffer;
        public const VkDebugReportObjectTypeEXT VK_DEBUG_REPORT_OBJECT_TYPE_IMAGE_EXT = VkDebugReportObjectTypeEXT.Image;
        public const VkDebugReportObjectTypeEXT VK_DEBUG_REPORT_OBJECT_TYPE_EVENT_EXT = VkDebugReportObjectTypeEXT.Event;
        public const VkDebugReportObjectTypeEXT VK_DEBUG_REPORT_OBJECT_TYPE_QUERY_POOL_EXT = VkDebugReportObjectTypeEXT.QueryPool;
        public const VkDebugReportObjectTypeEXT VK_DEBUG_REPORT_OBJECT_TYPE_BUFFER_VIEW_EXT = VkDebugReportObjectTypeEXT.BufferView;
        public const VkDebugReportObjectTypeEXT VK_DEBUG_REPORT_OBJECT_TYPE_IMAGE_VIEW_EXT = VkDebugReportObjectTypeEXT.ImageView;
        public const VkDebugReportObjectTypeEXT VK_DEBUG_REPORT_OBJECT_TYPE_SHADER_MODULE_EXT = VkDebugReportObjectTypeEXT.ShaderModule;
        public const VkDebugReportObjectTypeEXT VK_DEBUG_REPORT_OBJECT_TYPE_PIPELINE_CACHE_EXT = VkDebugReportObjectTypeEXT.PipelineCache;
        public const VkDebugReportObjectTypeEXT VK_DEBUG_REPORT_OBJECT_TYPE_PIPELINE_LAYOUT_EXT = VkDebugReportObjectTypeEXT.PipelineLayout;
        public const VkDebugReportObjectTypeEXT VK_DEBUG_REPORT_OBJECT_TYPE_RENDER_PASS_EXT = VkDebugReportObjectTypeEXT.RenderPass;
        public const VkDebugReportObjectTypeEXT VK_DEBUG_REPORT_OBJECT_TYPE_PIPELINE_EXT = VkDebugReportObjectTypeEXT.Pipeline;
        public const VkDebugReportObjectTypeEXT VK_DEBUG_REPORT_OBJECT_TYPE_DESCRIPTOR_SET_LAYOUT_EXT = VkDebugReportObjectTypeEXT.DescriptorSetLayout;
        public const VkDebugReportObjectTypeEXT VK_DEBUG_REPORT_OBJECT_TYPE_SAMPLER_EXT = VkDebugReportObjectTypeEXT.Sampler;
        public const VkDebugReportObjectTypeEXT VK_DEBUG_REPORT_OBJECT_TYPE_DESCRIPTOR_POOL_EXT = VkDebugReportObjectTypeEXT.DescriptorPool;
        public const VkDebugReportObjectTypeEXT VK_DEBUG_REPORT_OBJECT_TYPE_DESCRIPTOR_SET_EXT = VkDebugReportObjectTypeEXT.DescriptorSet;
        public const VkDebugReportObjectTypeEXT VK_DEBUG_REPORT_OBJECT_TYPE_FRAMEBUFFER_EXT = VkDebugReportObjectTypeEXT.Framebuffer;
        public const VkDebugReportObjectTypeEXT VK_DEBUG_REPORT_OBJECT_TYPE_COMMAND_POOL_EXT = VkDebugReportObjectTypeEXT.CommandPool;
        public const VkDebugReportObjectTypeEXT VK_DEBUG_REPORT_OBJECT_TYPE_SURFACE_KHR_EXT = VkDebugReportObjectTypeEXT.Surface;
        public const VkDebugReportObjectTypeEXT VK_DEBUG_REPORT_OBJECT_TYPE_SWAPCHAIN_KHR_EXT = VkDebugReportObjectTypeEXT.Swapchain;
        public const VkDebugReportObjectTypeEXT VK_DEBUG_REPORT_OBJECT_TYPE_DEBUG_REPORT_EXT = VkDebugReportObjectTypeEXT.DebugReport;
        public const VkDebugReportObjectTypeEXT VK_DEBUG_REPORT_OBJECT_TYPE_DISPLAY_KHR_EXT = VkDebugReportObjectTypeEXT.Display;
        public const VkDebugReportObjectTypeEXT VK_DEBUG_REPORT_OBJECT_TYPE_DISPLAY_MODE_KHR_EXT = VkDebugReportObjectTypeEXT.DisplayMode;
        public const VkDebugReportObjectTypeEXT VK_DEBUG_REPORT_OBJECT_TYPE_OBJECT_TABLE_NVX_EXT = VkDebugReportObjectTypeEXT.ObjectTable;
        public const VkDebugReportObjectTypeEXT VK_DEBUG_REPORT_OBJECT_TYPE_INDIRECT_COMMANDS_LAYOUT_NVX_EXT = VkDebugReportObjectTypeEXT.IndirectCommandsLayout;
    }

    public enum VkDebugReportErrorEXT
    {
        None = 0,
        CallbackRef = 1,
    }
    public static partial class RawConstants
    {
        public const VkDebugReportErrorEXT VK_DEBUG_REPORT_ERROR_NONE_EXT = VkDebugReportErrorEXT.None;
        public const VkDebugReportErrorEXT VK_DEBUG_REPORT_ERROR_CALLBACK_REF_EXT = VkDebugReportErrorEXT.CallbackRef;
    }

    public enum VkRasterizationOrderAMD
    {
        Strict = 0,
        Relaxed = 1,
    }
    public static partial class RawConstants
    {
        public const VkRasterizationOrderAMD VK_RASTERIZATION_ORDER_STRICT_AMD = VkRasterizationOrderAMD.Strict;
        public const VkRasterizationOrderAMD VK_RASTERIZATION_ORDER_RELAXED_AMD = VkRasterizationOrderAMD.Relaxed;
    }

    [Flags]
    public enum VkExternalMemoryHandleTypeFlagsNV
    {
        None = 0,
        OpaqueWin32 = 1,
        OpaqueWin32Kmt = 2,
        D3d11Image = 4,
        D3d11ImageKmt = 8,
    }
    public static partial class RawConstants
    {
        public const VkExternalMemoryHandleTypeFlagsNV VK_EXTERNAL_MEMORY_HANDLE_TYPE_OPAQUE_WIN32_BIT_NV = VkExternalMemoryHandleTypeFlagsNV.OpaqueWin32;
        public const VkExternalMemoryHandleTypeFlagsNV VK_EXTERNAL_MEMORY_HANDLE_TYPE_OPAQUE_WIN32_KMT_BIT_NV = VkExternalMemoryHandleTypeFlagsNV.OpaqueWin32Kmt;
        public const VkExternalMemoryHandleTypeFlagsNV VK_EXTERNAL_MEMORY_HANDLE_TYPE_D3D11_IMAGE_BIT_NV = VkExternalMemoryHandleTypeFlagsNV.D3d11Image;
        public const VkExternalMemoryHandleTypeFlagsNV VK_EXTERNAL_MEMORY_HANDLE_TYPE_D3D11_IMAGE_KMT_BIT_NV = VkExternalMemoryHandleTypeFlagsNV.D3d11ImageKmt;
    }

    [Flags]
    public enum VkExternalMemoryFeatureFlagsNV
    {
        None = 0,
        DedicatedOnly = 1,
        Exportable = 2,
        Importable = 4,
    }
    public static partial class RawConstants
    {
        public const VkExternalMemoryFeatureFlagsNV VK_EXTERNAL_MEMORY_FEATURE_DEDICATED_ONLY_BIT_NV = VkExternalMemoryFeatureFlagsNV.DedicatedOnly;
        public const VkExternalMemoryFeatureFlagsNV VK_EXTERNAL_MEMORY_FEATURE_EXPORTABLE_BIT_NV = VkExternalMemoryFeatureFlagsNV.Exportable;
        public const VkExternalMemoryFeatureFlagsNV VK_EXTERNAL_MEMORY_FEATURE_IMPORTABLE_BIT_NV = VkExternalMemoryFeatureFlagsNV.Importable;
    }

    public enum VkValidationCheckEXT
    {
        All = 0,
    }
    public static partial class RawConstants
    {
        public const VkValidationCheckEXT VK_VALIDATION_CHECK_ALL_EXT = VkValidationCheckEXT.All;
    }

    [Flags]
    public enum VkIndirectCommandsLayoutUsageFlagsNVX
    {
        None = 0,
        UnorderedSequences = 1,
        SparseSequences = 2,
        EmptyExecutions = 4,
        IndexedSequences = 8,
    }
    public static partial class RawConstants
    {
        public const VkIndirectCommandsLayoutUsageFlagsNVX VK_INDIRECT_COMMANDS_LAYOUT_USAGE_UNORDERED_SEQUENCES_BIT_NVX = VkIndirectCommandsLayoutUsageFlagsNVX.UnorderedSequences;
        public const VkIndirectCommandsLayoutUsageFlagsNVX VK_INDIRECT_COMMANDS_LAYOUT_USAGE_SPARSE_SEQUENCES_BIT_NVX = VkIndirectCommandsLayoutUsageFlagsNVX.SparseSequences;
        public const VkIndirectCommandsLayoutUsageFlagsNVX VK_INDIRECT_COMMANDS_LAYOUT_USAGE_EMPTY_EXECUTIONS_BIT_NVX = VkIndirectCommandsLayoutUsageFlagsNVX.EmptyExecutions;
        public const VkIndirectCommandsLayoutUsageFlagsNVX VK_INDIRECT_COMMANDS_LAYOUT_USAGE_INDEXED_SEQUENCES_BIT_NVX = VkIndirectCommandsLayoutUsageFlagsNVX.IndexedSequences;
    }

    [Flags]
    public enum VkObjectEntryUsageFlagsNVX
    {
        None = 0,
        Graphics = 1,
        Compute = 2,
    }
    public static partial class RawConstants
    {
        public const VkObjectEntryUsageFlagsNVX VK_OBJECT_ENTRY_USAGE_GRAPHICS_BIT_NVX = VkObjectEntryUsageFlagsNVX.Graphics;
        public const VkObjectEntryUsageFlagsNVX VK_OBJECT_ENTRY_USAGE_COMPUTE_BIT_NVX = VkObjectEntryUsageFlagsNVX.Compute;
    }

    public enum VkIndirectCommandsTokenTypeNVX
    {
        Pipeline = 0,
        DescriptorSet = 1,
        IndexBuffer = 2,
        VertexBuffer = 3,
        PushConstant = 4,
        DrawIndexed = 5,
        Draw = 6,
        Dispatch = 7,
    }
    public static partial class RawConstants
    {
        public const VkIndirectCommandsTokenTypeNVX VK_INDIRECT_COMMANDS_TOKEN_PIPELINE_NVX = VkIndirectCommandsTokenTypeNVX.Pipeline;
        public const VkIndirectCommandsTokenTypeNVX VK_INDIRECT_COMMANDS_TOKEN_DESCRIPTOR_SET_NVX = VkIndirectCommandsTokenTypeNVX.DescriptorSet;
        public const VkIndirectCommandsTokenTypeNVX VK_INDIRECT_COMMANDS_TOKEN_INDEX_BUFFER_NVX = VkIndirectCommandsTokenTypeNVX.IndexBuffer;
        public const VkIndirectCommandsTokenTypeNVX VK_INDIRECT_COMMANDS_TOKEN_VERTEX_BUFFER_NVX = VkIndirectCommandsTokenTypeNVX.VertexBuffer;
        public const VkIndirectCommandsTokenTypeNVX VK_INDIRECT_COMMANDS_TOKEN_PUSH_CONSTANT_NVX = VkIndirectCommandsTokenTypeNVX.PushConstant;
        public const VkIndirectCommandsTokenTypeNVX VK_INDIRECT_COMMANDS_TOKEN_DRAW_INDEXED_NVX = VkIndirectCommandsTokenTypeNVX.DrawIndexed;
        public const VkIndirectCommandsTokenTypeNVX VK_INDIRECT_COMMANDS_TOKEN_DRAW_NVX = VkIndirectCommandsTokenTypeNVX.Draw;
        public const VkIndirectCommandsTokenTypeNVX VK_INDIRECT_COMMANDS_TOKEN_DISPATCH_NVX = VkIndirectCommandsTokenTypeNVX.Dispatch;
    }

    public enum VkObjectEntryTypeNVX
    {
        DescriptorSet = 0,
        Pipeline = 1,
        IndexBuffer = 2,
        VertexBuffer = 3,
        PushConstant = 4,
    }
    public static partial class RawConstants
    {
        public const VkObjectEntryTypeNVX VK_OBJECT_ENTRY_DESCRIPTOR_SET_NVX = VkObjectEntryTypeNVX.DescriptorSet;
        public const VkObjectEntryTypeNVX VK_OBJECT_ENTRY_PIPELINE_NVX = VkObjectEntryTypeNVX.Pipeline;
        public const VkObjectEntryTypeNVX VK_OBJECT_ENTRY_INDEX_BUFFER_NVX = VkObjectEntryTypeNVX.IndexBuffer;
        public const VkObjectEntryTypeNVX VK_OBJECT_ENTRY_VERTEX_BUFFER_NVX = VkObjectEntryTypeNVX.VertexBuffer;
        public const VkObjectEntryTypeNVX VK_OBJECT_ENTRY_PUSH_CONSTANT_NVX = VkObjectEntryTypeNVX.PushConstant;
    }
}
#endif // CALLI_STUBS
