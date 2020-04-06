// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

using System;
using System.Numerics;

using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Processing.Processors.Effects;
using Xunit;

namespace SixLabors.ImageSharp.Tests.Processing.Processors.Effects
{
    [GroupOutput("Effects")]
    public class PixelShaderTest
    {
        [Theory]
        [WithFile(TestImages.Png.CalliphoraPartial, PixelTypes.Rgba32)]
        public void FullImage<TPixel>(TestImageProvider<TPixel> provider)
            where TPixel : unmanaged, IPixel<TPixel>
        {
            provider.RunValidatingProcessorTest(
                x => x.ProcessPixelRowsAsVector4(
                span => default(PixelAverageProcessor).Invoke(span)),
                appendPixelTypeToFileName: false);
        }

        [Theory]
        [WithFile(TestImages.Png.CalliphoraPartial, PixelTypes.Rgba32)]
        public void InBox<TPixel>(TestImageProvider<TPixel> provider)
            where TPixel : unmanaged, IPixel<TPixel>
        {
            provider.RunRectangleConstrainedValidatingProcessorTest(
                (x, rect) => x.ProcessPixelRowsAsVector4(
                    span => default(PixelAverageProcessor).Invoke(span), rect));
        }

        private readonly struct PixelAverageProcessor : IPixelRowDelegate
        {
            public void Invoke(Span<Vector4> span)
            {
                for (int i = 0; i < span.Length; i++)
                {
                    Vector4 v4 = span[i];
                    float avg = (v4.X + v4.Y + v4.Z) / 3f;
                    span[i] = new Vector4(avg);
                }
            }
        }

        [Theory]
        [WithFile(TestImages.Png.CalliphoraPartial, PixelTypes.Rgba32)]
        public void PositionAwareFullImage<TPixel>(TestImageProvider<TPixel> provider)
            where TPixel : unmanaged, IPixel<TPixel>
        {
            provider.RunValidatingProcessorTest(
                c => c.ProcessPositionAwarePixelRowsAsVector4(
                    (span, offset) => default(TrigonometryProcessor).Invoke(span, offset)),
                appendPixelTypeToFileName: false);
        }

        [Theory]
        [WithFile(TestImages.Png.CalliphoraPartial, PixelTypes.Rgba32)]
        public void PositionAwareInBox<TPixel>(TestImageProvider<TPixel> provider)
            where TPixel : unmanaged, IPixel<TPixel>
        {
            provider.RunRectangleConstrainedValidatingProcessorTest(
                (c, rect) => c.ProcessPositionAwarePixelRowsAsVector4(
                    (span, offset) => default(TrigonometryProcessor).Invoke(span, offset), rect));
        }

        private readonly struct TrigonometryProcessor : IPixelRowDelegate<Point>
        {
            public void Invoke(Span<Vector4> span, Point value)
            {
                int y = value.Y;
                int x = value.X;
                for (int i = 0; i < span.Length; i++)
                {
                    float
                        sine = MathF.Sin(y),
                        cosine = MathF.Cos(x + i),
                        sum = sine + cosine,
                        abs = MathF.Abs(sum),
                        a = 0.5f + (abs / 2);

                    Vector4 v4 = span[i];
                    float avg = (v4.X + v4.Y + v4.Z) / 3f;
                    var gray = new Vector4(avg, avg, avg, a);

                    span[i] = Vector4.Clamp(gray, Vector4.Zero, Vector4.One);
                }
            }
        }
    }
}