using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Vulkan
{
    [StructLayout(LayoutKind.Sequential)]
    public struct VkColor4
    {
        /// <summary>
        /// Gets a <see cref="VkColor4"/> with all of its components set to zero.
        /// </summary>
        public static readonly VkColor4 Zero;

        /// <summary>
        /// The red component of the color.
        /// </summary>
        public float R;
        /// <summary>
        /// The green component of the color.
        /// </summary>
        public float G;
        /// <summary>
        /// The blue component of the color.
        /// </summary>
        public float B;
        /// <summary>
        /// The alpha component of the color.
        /// </summary>
        public float A;

        /// <summary>
        /// Initializes a new instance of the <see cref="VkColor4"/> structure.
        /// </summary>
        /// <param name="r">The red component.</param>
        /// <param name="g">The green component.</param>
        /// <param name="b">The blue component.</param>
        /// <param name="a">The alpha component.</param>
        public VkColor4(float r, float g, float b, float a)
        {
            R = r;
            G = g;
            B = b;
            A = a;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct VkColorI4
    {
        /// <summary>
        /// Gets a <see cref="VkColorI4"/> with all of its components set to zero.
        /// </summary>
        public static readonly VkColorI4 Zero;

        /// <summary>
        /// The red component of the color.
        /// </summary>
        public int R;
        /// <summary>
        /// The green component of the color.
        /// </summary>
        public int G;
        /// <summary>
        /// The blue component of the color.
        /// </summary>
        public int B;
        /// <summary>
        /// The alpha component of the color.
        /// </summary>
        public int A;

        /// <summary>
        /// Initializes a new instance of the <see cref="VkColorI4"/> structure.
        /// </summary>
        /// <param name="r">The red component.</param>
        /// <param name="g">The green component.</param>
        /// <param name="b">The blue component.</param>
        /// <param name="a">The alpha component.</param>
        public VkColorI4(int r, int g, int b, int a)
        {
            R = r;
            G = g;
            B = b;
            A = a;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct VkColorU4
    {
        /// <summary>
        /// Gets a <see cref="VkColorU4"/> with all of its components set to zero.
        /// </summary>
        public static readonly VkColorU4 Zero;

        /// <summary>
        /// The red component of the color.
        /// </summary>
        public uint R;
        /// <summary>
        /// The green component of the color.
        /// </summary>
        public uint G;
        /// <summary>
        /// The blue component of the color.
        /// </summary>
        public uint B;
        /// <summary>
        /// The alpha component of the color.
        /// </summary>
        public uint A;

        /// <summary>
        /// Initializes a new instance of the <see cref="VkColorU4"/> structure.
        /// </summary>
        /// <param name="r">The red component.</param>
        /// <param name="g">The green component.</param>
        /// <param name="b">The blue component.</param>
        /// <param name="a">The alpha component.</param>
        public VkColorU4(uint r, uint g, uint b, uint a)
        {
            R = r;
            G = g;
            B = b;
            A = a;
        }
    }
    }
