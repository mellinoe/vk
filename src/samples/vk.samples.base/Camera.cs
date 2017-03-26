// This code has been adapted from the "Vulkan" C++ example repository, by Sascha Willems: https://github.com/SaschaWillems/Vulkan
// It is a direct translation from the original C++ code and style, with as little transformation as possible.

// Original file: base/camera.hpp, 

/*
* Vulkan Example base class
*
* Copyright (C) 2016 by Sascha Willems - www.saschawillems.de
*
* This code is licensed under the MIT license (MIT) (http://opensource.org/licenses/MIT)
*/

using System;
using System.Numerics;

namespace Vk.Samples
{
    public class Camera
    {
        float fov;
        float znear, zfar;

        public void updateViewMatrix()
        {
            Matrix4x4 rotM = Matrix4x4.Identity;
            Matrix4x4 transM;

            rotM = Matrix4x4.CreateRotationX(Util.DegreesToRadians(rotation.X)) * rotM;
            rotM = Matrix4x4.CreateRotationY(Util.DegreesToRadians(rotation.Y)) * rotM;
            rotM = Matrix4x4.CreateRotationZ(Util.DegreesToRadians(rotation.Z)) * rotM;

            transM = Matrix4x4.CreateTranslation(position);

            //if (type == CameraType.firstperson)
            {
                matrices_view = rotM * transM;
            }
            //else
            //{
            //    matrices_view = transM * rotM;
            //}
        }

        public enum CameraType { lookat, firstperson }
        public CameraType type = CameraType.lookat;

        Vector3 rotation = new Vector3();
        Vector3 position = new Vector3();

        public float rotationSpeed = 1.0f;
        public float movementSpeed = 1.0f;

        public Matrix4x4 matrices_perspective;
        public Matrix4x4 matrices_view;

        bool keys_left = false;
        bool keys_right = false;
        bool keys_up = false;
        bool keys_down = false;

        public bool moving()
        {
            return keys_left || keys_right || keys_up || keys_down;
        }

        public void setPerspective(float fov, float aspect, float znear, float zfar)
        {
            this.fov = fov;
            this.znear = znear;
            this.zfar = zfar;
            matrices_perspective = Matrix4x4.CreatePerspectiveFieldOfView(Util.DegreesToRadians(fov), aspect, znear, zfar);
        }

        public void updateAspectRatio(float aspect)
        {
            matrices_perspective = Matrix4x4.CreatePerspectiveFieldOfView(Util.DegreesToRadians(fov), aspect, znear, zfar);
        }

        public void setPosition(Vector3 position)
        {
            this.position = position;
            updateViewMatrix();
        }

        public void setRotation(Vector3 rotation)
        {
            this.rotation = rotation;
            updateViewMatrix();
        }

        public void rotate(Vector3 delta)
        {
            this.rotation += delta;
            updateViewMatrix();
        }

        public void setTranslation(Vector3 translation)
        {
            this.position = translation;
            updateViewMatrix();
        }

        public void translate(Vector3 delta)
        {
            this.position += delta;
            updateViewMatrix();
        }

        public void update(float deltaTime)
        {
            if (type == CameraType.firstperson)
            {
                if (moving())
                {
                    Vector3 camFront;
                    camFront.X = (float)(-Math.Cos(Util.DegreesToRadians(rotation.X)) * Math.Sin(Util.DegreesToRadians(rotation.Y)));
                    camFront.Y = (float)(Math.Sin(Util.DegreesToRadians(rotation.X)));
                    camFront.Z = (float)(Math.Cos(Util.DegreesToRadians(rotation.X)) * Math.Cos(Util.DegreesToRadians(rotation.Y)));
                    camFront = Vector3.Normalize(camFront);

                    float moveSpeed = deltaTime * movementSpeed;

                    if (keys_up)
                        position += camFront * moveSpeed;
                    if (keys_down)
                        position -= camFront * moveSpeed;
                    if (keys_left)
                        position -= Vector3.Normalize(Vector3.Cross(camFront, new Vector3(0.0f, 1.0f, 0.0f))) * moveSpeed;
                    if (keys_right)
                        position += Vector3.Normalize(Vector3.Cross(camFront, new Vector3(0.0f, 1.0f, 0.0f))) * moveSpeed;

                    updateViewMatrix();
                }
            }
        }

        // Update camera passing separate axis data (gamepad)
        // Returns true if view or position has been changed
        bool updatePad(Vector2 axisLeft, Vector2 axisRight, float deltaTime)
        {
            bool retVal = false;

            if (type == CameraType.firstperson)
            {
                // Use the common console thumbstick layout		
                // Left = view, right = move

                const float deadZone = 0.0015f;
                const float range = 1.0f - deadZone;

                Vector3 camFront;
                camFront.X = (float)(-Math.Cos(Util.DegreesToRadians(rotation.X)) * Math.Sin(Util.DegreesToRadians(rotation.Y)));
                camFront.Y = (float)(Math.Sin(Util.DegreesToRadians(rotation.X)));
                camFront.Z = (float)(Math.Cos(Util.DegreesToRadians(rotation.X)) * Math.Cos(Util.DegreesToRadians(rotation.Y)));
                camFront = Vector3.Normalize(camFront);

                float moveSpeed = deltaTime * movementSpeed * 2.0f;
                float rotSpeed = deltaTime * rotationSpeed * 50.0f;

                // Move
                if (Math.Abs(axisLeft.Y) > deadZone)
                {
                    float pos = (Math.Abs(axisLeft.Y) - deadZone) / range;
                    position -= camFront * pos * ((axisLeft.Y < 0.0f) ? -1.0f : 1.0f) * moveSpeed;
                    retVal = true;
                }
                if (Math.Abs(axisLeft.X) > deadZone)
                {
                    float pos = (Math.Abs(axisLeft.X) - deadZone) / range;
                    position += Vector3.Normalize(Vector3.Cross(camFront, new Vector3(0.0f, 1.0f, 0.0f))) * pos * ((axisLeft.X < 0.0f) ? -1.0f : 1.0f) * moveSpeed;
                    retVal = true;
                }

                // Rotate
                if (Math.Abs(axisRight.X) > deadZone)
                {
                    float pos = (Math.Abs(axisRight.X) - deadZone) / range;
                    rotation.Y += pos * ((axisRight.X < 0.0f) ? -1.0f : 1.0f) * rotSpeed;
                    retVal = true;
                }
                if (Math.Abs(axisRight.Y) > deadZone)
                {
                    float pos = (Math.Abs(axisRight.Y) - deadZone) / range;
                    rotation.X -= pos * ((axisRight.Y < 0.0f) ? -1.0f : 1.0f) * rotSpeed;
                    retVal = true;
                }
            }
            else
            {
                // todo: move code from example base class for look-at
            }

            if (retVal)
            {
                updateViewMatrix();
            }

            return retVal;
        }
    }
}
