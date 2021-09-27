using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DotRPG.Objects.Complexity
{
    public class Bone
    {
        public Single Rotation;
        public Vector2 RotationOrigin;
        public Vector2 SubnodeOrigin;
        public Single Length;
        public Texture2D LimbTexture;

        public List<Bone> Subnodes = new List<Bone>();
        /// <summary>
        /// 2D vector that corresponds to bone's alignment.
        /// </summary>
        public Vector2 Alignment
        {
            get
            {
                return new Vector2
                (
                    Length * (float)Math.Cos(Rotation * Math.PI / 180.0f),
                    Length * (float)Math.Sin(Rotation * Math.PI / 180.0f)
                );
            }
        }
    }
}
