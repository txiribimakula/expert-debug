﻿namespace Txiribimakula.ExpertWatch.Drawing
{
    public class GeometryDrawer
    {
        public DrawableVisitor DrawableVisitor { get; set; }

        public GeometryDrawer(DrawableVisitor visitor) {
            DrawableVisitor = visitor;
        }

        public void TransformGeometries(DrawableCollection drawables) {
            foreach (var drawable in drawables) {
                drawable.TransformGeometry(DrawableVisitor);
            }
        }

        public void TransformGeometry(IDrawable drawable) {
            drawable.TransformGeometry(DrawableVisitor);
        }
    }
}
