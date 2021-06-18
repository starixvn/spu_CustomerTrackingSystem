// --------------------------------------------------------------------------
// <copyright file="TextBlock.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Jason.Ly" date="2014.6.11">Created</change>
// </history>

namespace Fanex.NAP.TGRS.Core.ExcelUtility.RichTextExtensions
{
    using NPOI.SS.UserModel;

    /// <summary>
    /// Wrapped model for a formatted text in a cell.
    /// </summary>
    public class TextBlock
    {
        private string _text;
        private IFont _font;

        public TextBlock(string text)
        {
            this._text = text;
        }

        public TextBlock(string text, IFont font)
        {
            this._text = text;
            this._font = font;
        }

        public string Text
        {
            get
            {
                return this._text;
            }
        }

        public IFont Font
        {
            get
            {
                return this._font;
            }
        }
    }
}