﻿// Copyright (c) Six Labors and contributors.
// Licensed under the GNU Affero General Public License, Version 3.

using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Processing.Processors.Transforms;
using Xunit;

namespace SixLabors.ImageSharp.Tests.Processing.Transforms
{
    public class EntropyCropTest : BaseImageOperationsExtensionTest
    {
        [Theory]
        [InlineData(0.5F)]
        [InlineData(.2F)]
        public void EntropyCropThresholdFloatEntropyCropProcessorWithThreshold(float threshold)
        {
            this.operations.EntropyCrop(threshold);
            EntropyCropProcessor processor = this.Verify<EntropyCropProcessor>();

            Assert.Equal(threshold, processor.Threshold);
        }
    }
}