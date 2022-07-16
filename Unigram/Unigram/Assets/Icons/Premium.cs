﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//       LottieGen version:
//           7.1.0-build.5+g109463c06a
//       
//       Command:
//           LottieGen -Language CSharp -Namespace Unigram.Assets.Icons -Public -WinUIVersion 2.7 -InputFile Premium.json
//       
//       Input file:
//           Premium.json (2349 bytes created 13:21+02:00 Jun 12 2022)
//       
//       LottieGen source:
//           http://aka.ms/Lottie
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
// ____________________________________
// |       Object stats       | Count |
// |__________________________|_______|
// | All CompositionObjects   |    28 |
// |--------------------------+-------|
// | Expression animators     |     1 |
// | KeyFrame animators       |     1 |
// | Reference parameters     |     1 |
// | Expression operations    |     0 |
// |--------------------------+-------|
// | Animated brushes         |     - |
// | Animated gradient stops  |     - |
// | ExpressionAnimations     |     1 |
// | PathKeyFrameAnimations   |     - |
// |--------------------------+-------|
// | ContainerVisuals         |     1 |
// | ShapeVisuals             |     1 |
// |--------------------------+-------|
// | ContainerShapes          |     1 |
// | CompositionSpriteShapes  |     1 |
// |--------------------------+-------|
// | Brushes                  |     1 |
// | Gradient stops           |     3 |
// | CompositionVisualSurface |     - |
// ------------------------------------
using Microsoft.Graphics.Canvas.Geometry;
using System;
using System.Collections.Generic;
using System.Numerics;
using Windows.Graphics;
using Windows.UI;
using Windows.UI.Composition;

namespace Unigram.Assets.Icons
{
    // Name:        u_premium
    // Frame rate:  60 fps
    // Frame count: 30
    // Duration:    500.0 mS
    public sealed class Premium
        : Microsoft.UI.Xaml.Controls.IAnimatedVisualSource
        , Microsoft.UI.Xaml.Controls.IAnimatedVisualSource2
    {
        // Animation duration: 0.500 seconds.
        internal const long c_durationTicks = 5000000;

        public Microsoft.UI.Xaml.Controls.IAnimatedVisual TryCreateAnimatedVisual(Compositor compositor)
        {
            object ignored = null;
            return TryCreateAnimatedVisual(compositor, out ignored);
        }

        public Microsoft.UI.Xaml.Controls.IAnimatedVisual TryCreateAnimatedVisual(Compositor compositor, out object diagnostics)
        {
            diagnostics = null;

            if (Premium_AnimatedVisual.IsRuntimeCompatible())
            {
                var res = 
                    new Premium_AnimatedVisual(
                        compositor
                        );
                    res.CreateAnimations();
                    return res;
            }

            return null;
        }

        /// <summary>
        /// Gets the number of frames in the animation.
        /// </summary>
        public double FrameCount => 30d;

        /// <summary>
        /// Gets the frame rate of the animation.
        /// </summary>
        public double Framerate => 60d;

        /// <summary>
        /// Gets the duration of the animation.
        /// </summary>
        public TimeSpan Duration => TimeSpan.FromTicks(c_durationTicks);

        /// <summary>
        /// Converts a zero-based frame number to the corresponding progress value denoting the
        /// start of the frame.
        /// </summary>
        public double FrameToProgress(double frameNumber)
        {
            return frameNumber / 30d;
        }

        /// <summary>
        /// Returns a map from marker names to corresponding progress values.
        /// </summary>
        public IReadOnlyDictionary<string, double> Markers =>
            new Dictionary<string, double>
            {
                { "NormalToPointerOver_Start", 0.0 },
                { "NormalToPointerOver_End", 1 },
            };

        /// <summary>
        /// Sets the color property with the given name, or does nothing if no such property
        /// exists.
        /// </summary>
        public void SetColorProperty(string propertyName, Color value)
        {
        }

        /// <summary>
        /// Sets the scalar property with the given name, or does nothing if no such property
        /// exists.
        /// </summary>
        public void SetScalarProperty(string propertyName, double value)
        {
        }

        sealed class Premium_AnimatedVisual : Microsoft.UI.Xaml.Controls.IAnimatedVisual
        {
            const long c_durationTicks = 5000000;
            readonly Compositor _c;
            readonly ExpressionAnimation _reusableExpressionAnimation;
            CompositionContainerShape _containerShape;
            ContainerVisual _root;
            CubicBezierEasingFunction _cubicBezierEasingFunction_0;

            void BindProperty(
                CompositionObject target,
                string animatedPropertyName,
                string expression,
                string referenceParameterName,
                CompositionObject referencedObject)
            {
                _reusableExpressionAnimation.ClearAllParameters();
                _reusableExpressionAnimation.Expression = expression;
                _reusableExpressionAnimation.SetReferenceParameter(referenceParameterName, referencedObject);
                target.StartAnimation(animatedPropertyName, _reusableExpressionAnimation);
            }

            Vector2KeyFrameAnimation CreateVector2KeyFrameAnimation(float initialProgress, Vector2 initialValue, CompositionEasingFunction initialEasingFunction)
            {
                var result = _c.CreateVector2KeyFrameAnimation();
                result.Duration = TimeSpan.FromTicks(c_durationTicks);
                result.InsertKeyFrame(initialProgress, initialValue, initialEasingFunction);
                return result;
            }

            CompositionSpriteShape CreateSpriteShape(CompositionGeometry geometry, Matrix3x2 transformMatrix, CompositionBrush fillBrush)
            {
                var result = _c.CreateSpriteShape(geometry);
                result.TransformMatrix = transformMatrix;
                result.FillBrush = fillBrush;
                return result;
            }

            // - - - - Shape tree root for layer: Shape
            // - - ShapeGroup: Shape Scale:10,10
            CanvasGeometry Geometry()
            {
                CanvasGeometry result;
                using (var builder = new CanvasPathBuilder(null))
                {
                    builder.SetFilledRegionDetermination(CanvasFilledRegionDetermination.Winding);
                    builder.BeginFigure(new Vector2(0.897000015F, -7.10099983F));
                    builder.AddCubicBezier(new Vector2(0.529999971F, -7.84399986F), new Vector2(-0.529999971F, -7.84399986F), new Vector2(-0.897000015F, -7.10099983F));
                    builder.AddLine(new Vector2(-2.5940001F, -3.66100001F));
                    builder.AddCubicBezier(new Vector2(-2.74000001F, -3.36599994F), new Vector2(-3.02099991F, -3.16100001F), new Vector2(-3.34699988F, -3.11400008F));
                    builder.AddLine(new Vector2(-7.14400005F, -2.56200004F));
                    builder.AddCubicBezier(new Vector2(-7.96400023F, -2.44300008F), new Vector2(-8.29199982F, -1.43599999F), new Vector2(-7.69799995F, -0.856999993F));
                    builder.AddLine(new Vector2(-7.03000021F, -0.206F));
                    builder.AddCubicBezier(new Vector2(-6.07399988F, 0.726000011F), new Vector2(-4.71799994F, 1.11899996F), new Vector2(-3.41199994F, 0.842999995F));
                    builder.AddLine(new Vector2(0.342999995F, 0.0500000007F));
                    builder.AddCubicBezier(new Vector2(0.901000023F, -0.0680000037F), new Vector2(1.16299999F, 0.713F), new Vector2(0.646000028F, 0.954999983F));
                    builder.AddLine(new Vector2(-2.90400004F, 2.61400008F));
                    builder.AddCubicBezier(new Vector2(-4.09100008F, 3.16899991F), new Vector2(-4.93100023F, 4.26999998F), new Vector2(-5.15299988F, 5.5619998F));
                    builder.AddLine(new Vector2(-5.3119998F, 6.48699999F));
                    builder.AddCubicBezier(new Vector2(-5.45200014F, 7.3039999F), new Vector2(-4.59499979F, 7.92700005F), new Vector2(-3.86100006F, 7.54099989F));
                    builder.AddLine(new Vector2(-0.465000004F, 5.75600004F));
                    builder.AddCubicBezier(new Vector2(-0.173999995F, 5.60300016F), new Vector2(0.173999995F, 5.60300016F), new Vector2(0.465000004F, 5.75600004F));
                    builder.AddLine(new Vector2(3.86100006F, 7.54099989F));
                    builder.AddCubicBezier(new Vector2(4.59499979F, 7.92700005F), new Vector2(5.45200014F, 7.3039999F), new Vector2(5.3119998F, 6.48699999F));
                    builder.AddLine(new Vector2(4.66300011F, 2.70600009F));
                    builder.AddCubicBezier(new Vector2(4.60699987F, 2.38199997F), new Vector2(4.71500015F, 2.05100012F), new Vector2(4.95100021F, 1.82099998F));
                    builder.AddLine(new Vector2(7.69799995F, -0.856999993F));
                    builder.AddCubicBezier(new Vector2(8.29199982F, -1.43599999F), new Vector2(7.96400023F, -2.44300008F), new Vector2(7.14400005F, -2.56200004F));
                    builder.AddLine(new Vector2(3.34699988F, -3.11400008F));
                    builder.AddCubicBezier(new Vector2(3.02099991F, -3.16100001F), new Vector2(2.74099994F, -3.36599994F), new Vector2(2.59500003F, -3.66100001F));
                    builder.AddLine(new Vector2(0.897000015F, -7.10099983F));
                    builder.EndFigure(CanvasFigureLoop.Closed);
                    result = CanvasGeometry.CreatePath(builder);
                }
                return result;
            }

            // - - - Shape tree root for layer: Shape
            // - ShapeGroup: Shape Scale:10,10
            // Stop 0
            CompositionColorGradientStop GradientStop_0_AlmostCornflowerBlue_FF6B92FF()
            {
                return _c.CreateColorGradientStop(0F, Color.FromArgb(0xFF, 0x6B, 0x92, 0xFF));
            }

            // - - - Shape tree root for layer: Shape
            // - ShapeGroup: Shape Scale:10,10
            // Stop 1
            CompositionColorGradientStop GradientStop_0p484_AlmostMediumSlateBlue_FF966EFF()
            {
                return _c.CreateColorGradientStop(0.483999997F, Color.FromArgb(0xFF, 0x96, 0x6E, 0xFF));
            }

            // - - - Shape tree root for layer: Shape
            // - ShapeGroup: Shape Scale:10,10
            // Stop 2
            CompositionColorGradientStop GradientStop_1_AlmostOrchid_FFE36ACE()
            {
                return _c.CreateColorGradientStop(1F, Color.FromArgb(0xFF, 0xE3, 0x6A, 0xCE));
            }

            // Shape tree root for layer: Shape
            CompositionContainerShape ContainerShape()
            {
                if (_containerShape != null) { return _containerShape; }
                var result = _containerShape = _c.CreateContainerShape();
                result.Offset = new Vector2(100.011002F, 100F);
                // ShapeGroup: Shape Scale:10,10
                result.Shapes.Add(SpriteShape());
                return result;
            }

            // - - Shape tree root for layer: Shape
            // ShapeGroup: Shape Scale:10,10
            CompositionLinearGradientBrush LinearGradientBrush()
            {
                var result = _c.CreateLinearGradientBrush();
                var colorStops = result.ColorStops;
                colorStops.Add(GradientStop_0_AlmostCornflowerBlue_FF6B92FF());
                colorStops.Add(GradientStop_0p484_AlmostMediumSlateBlue_FF966EFF());
                colorStops.Add(GradientStop_1_AlmostOrchid_FFE36ACE());
                result.MappingMode = CompositionMappingMode.Absolute;
                result.StartPoint = new Vector2(-5.00099993F, 8F);
                result.EndPoint = new Vector2(4.99900007F, -5F);
                return result;
            }

            // - - Shape tree root for layer: Shape
            // ShapeGroup: Shape Scale:10,10
            CompositionPathGeometry PathGeometry()
            {
                return _c.CreatePathGeometry(new CompositionPath(Geometry()));
            }

            // - Shape tree root for layer: Shape
            // Path 1
            CompositionSpriteShape SpriteShape()
            {
                // Scale:<10, 10>
                var geometry = PathGeometry();
                var result = CreateSpriteShape(geometry, new Matrix3x2(10F, 0F, 0F, 10F, 0F, 0F), LinearGradientBrush());;
                return result;
            }

            // The root of the composition.
            ContainerVisual Root()
            {
                if (_root != null) { return _root; }
                var result = _root = _c.CreateContainerVisual();
                var propertySet = result.Properties;
                propertySet.InsertScalar("Progress", 0F);
                // Shape tree root for layer: Shape
                result.Children.InsertAtTop(ShapeVisual_0());
                return result;
            }

            CubicBezierEasingFunction CubicBezierEasingFunction_0()
            {
                return (_cubicBezierEasingFunction_0 == null)
                    ? _cubicBezierEasingFunction_0 = _c.CreateCubicBezierEasingFunction(new Vector2(0.600000024F, 0F), new Vector2(0.400000006F, 1F))
                    : _cubicBezierEasingFunction_0;
            }

            // Shape tree root for layer: Shape
            ShapeVisual ShapeVisual_0()
            {
                var result = _c.CreateShapeVisual();
                result.Size = new Vector2(200F, 200F);
                result.Shapes.Add(ContainerShape());
                return result;
            }

            // - - Shape tree root for layer: Shape
            // Scale
            StepEasingFunction HoldThenStepEasingFunction()
            {
                var result = _c.CreateStepEasingFunction();
                result.IsFinalStepSingleFrame = true;
                return result;
            }

            // - Shape tree root for layer: Shape
            // Scale
            Vector2KeyFrameAnimation ScaleVector2Animation()
            {
                // Frame 0.
                var result = CreateVector2KeyFrameAnimation(0F, new Vector2(1F, 1F), HoldThenStepEasingFunction());
                // Frame 8.
                result.InsertKeyFrame(0.266666681F, new Vector2(1.12F, 1.12F), CubicBezierEasingFunction_0());
                // Frame 16.
                result.InsertKeyFrame(0.533333361F, new Vector2(0.949999988F, 0.949999988F), CubicBezierEasingFunction_0());
                // Frame 24.
                result.InsertKeyFrame(0.800000012F, new Vector2(1F, 1F), CubicBezierEasingFunction_0());
                return result;
            }

            internal Premium_AnimatedVisual(
                Compositor compositor
                )
            {
                _c = compositor;
                _reusableExpressionAnimation = compositor.CreateExpressionAnimation();
                Root();
            }

            public Visual RootVisual => _root;
            public TimeSpan Duration => TimeSpan.FromTicks(c_durationTicks);
            public Vector2 Size => new Vector2(200F, 200F);
            void IDisposable.Dispose() => _root?.Dispose();

            public void CreateAnimations()
            {
                _containerShape.StartAnimation("Scale", ScaleVector2Animation());
                var controller = _containerShape.TryGetAnimationController("Scale");
                controller.Pause();
                BindProperty(controller, "Progress", "_.Progress", "_", _root);
            }

            public void DestroyAnimations()
            {
                _containerShape.StopAnimation("Scale");
            }

            internal static bool IsRuntimeCompatible()
            {
                return Windows.Foundation.Metadata.ApiInformation.IsApiContractPresent("Windows.Foundation.UniversalApiContract", 7);
            }
        }
    }
}