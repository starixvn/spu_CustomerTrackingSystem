// --------------------------------------------------------------------------
// <copyright file="RichTextRender.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Jason.Ly" date="2014.6.11">Created</change>
// </history>

namespace Fanex.NAP.TGRS.Core.ExcelUtility.RichTextExtensions
{
    using System.Collections.Generic;
    using System.Text;
    using NPOI.HSSF.UserModel;
    using NPOI.SS.UserModel;

    /// <summary>
    /// Rendering rich text helper.
    /// </summary>
    public class RichTextRender
    {
        private IFont _defaultFont;
        private List<TextBlock> _blocks;

        public RichTextRender()
        {
            this._blocks = new List<TextBlock>();
        }

        public RichTextRender(IFont defaultFont)
            : this()
        {
            this._defaultFont = defaultFont;
        }

        /// <summary>
        /// Clears all previous added blocks.
        /// </summary>
        /// <returns>RichTextRender object.</returns>
        public RichTextRender Clear()
        {
            this._blocks.Clear();

            return this;
        }

        /// <summary>
        /// Adds an unformatted text.
        /// </summary>
        /// <param name="text">Unformatted text.</param>
        /// <returns>RichTextRender object.</returns>
        public RichTextRender AddText(string text)
        {
            this._blocks.Add(new TextBlock(text));

            return this;
        }

        /// <summary>
        /// Adds a text and its associate font to the block list.
        /// </summary>
        /// <param name="text">The text that needs to be added.</param>
        /// <param name="font">The font of the added text.</param>
        /// <returns>RichTextRender object.</returns>
        public RichTextRender AddText(string text, IFont font)
        {
            this._blocks.Add(new TextBlock(text, font));

            return this;
        }

        /// <summary>
        /// Adds a text block.
        /// </summary>
        /// <param name="block">The text block.</param>
        /// <returns>RichTextRender object.</returns>
        public RichTextRender AddText(TextBlock block)
        {
            this._blocks.Add(block);

            return this;
        }

        /// <summary>
        /// Converts added blocks to IRichTextString.
        /// </summary>
        /// <returns>IRichTextString object.</returns>
        public IRichTextString Render()
        {
            StringBuilder textBuilder = new StringBuilder();
            IRichTextString result = null;
            IFont currentFont = null;
            int textIndex = 0;

            foreach (TextBlock block in this._blocks)
            {
                textBuilder.Append(block.Text);
            }

            result = new HSSFRichTextString(textBuilder.ToString());

            foreach (TextBlock block in this._blocks)
            {
                currentFont = block.Font ?? _defaultFont;

                if (currentFont != null)
                {
                    result.ApplyFont(textIndex, textIndex + block.Text.Length, currentFont);
                }

                textIndex += block.Text.Length;
            }

            return result;
        }
    }
}