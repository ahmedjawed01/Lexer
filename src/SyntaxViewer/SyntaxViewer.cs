// Copyright (c) Adnan Umer. All rights reserved. Follow me @aztnan
// Email: aztnan@outlook.com
//
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.


using Lexer;
using System.Collections.Generic;
using System.Text;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Media;

namespace SyntaxViewer
{
    public class SyntaxViewer : Control
    {
        public SyntaxViewer()
        {
            DefaultStyleKey = typeof(SyntaxViewer);

            TextView = new RichTextBlock { /* FontSize = 13, FontFamily = new FontFamily("Consolas") */ };
            LineNumberBlock = new TextBlock { /*FontSize = 13, FontFamily = new FontFamily("Consolas"),  */ Foreground = new SolidColorBrush(Color.FromArgb(255, 43, 145, 175)) };

        }

        #region TextView

        public static DependencyProperty TextViewProperty =
            DependencyProperty.Register("TextView", typeof(RichTextBlock), typeof(SyntaxViewer),
                                        new PropertyMetadata(null));

        public RichTextBlock TextView
        {
            get { return (RichTextBlock)GetValue(TextViewProperty); }
            set { SetValue(TextViewProperty, value); }
        }

        #endregion

        #region Line No Margin

        public static DependencyProperty LineNumberBlockProperty =
            DependencyProperty.Register("LineNumberBlock", typeof(TextBlock), typeof(SyntaxViewer),
                                        new PropertyMetadata(null));

        public TextBlock LineNumberBlock
        {
            get { return (TextBlock)GetValue(LineNumberBlockProperty); }
            set { SetValue(LineNumberBlockProperty, value); }
        }

        #endregion

        #region Text

        public static DependencyProperty TextProperty = DependencyProperty.Register("Text", typeof(string), typeof(SyntaxViewer), new PropertyMetadata("", OnTextProperyChanged));

        private static void OnTextProperyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((SyntaxViewer)d).OnTextChanged((string)e.NewValue);
        }

        public string Text
        {
            get { return GetValue(TextProperty).ToString(); }
            set { SetValue(TextProperty, value); }
        }

        Tokenizer lexer = new Tokenizer(new Lexer.Grammers.PythonGrammer());

        Dictionary<TokenType, Brush> ColorDict = new Dictionary<TokenType, Brush>()
        {
            { TokenType.Comment, new SolidColorBrush(Color.FromArgb(255, 221, 0, 0)) },
            { TokenType.String, new SolidColorBrush(Color.FromArgb(255, 0, 171, 0)) },
            { TokenType.Builtins, new SolidColorBrush(Color.FromArgb(255, 144, 0, 144)) },
            { TokenType.Keyword, new SolidColorBrush(Color.FromArgb(255, 255, 119, 0)) },
        };

        int lines;
        protected void OnTextChanged(string value)
        {
            if (TextView == null || value == null) return;

            lines = value.Split('\n').Length;
            StringBuilder builder = new StringBuilder();
            for (int i = 1; i <= lines; i++)
            {
                builder.Append(i);
                builder.Append("\n");
            }

            LineNumberBlock.Text = builder.ToString();
            builder.Clear();

            TextView.Blocks.Clear();
            var p = new Paragraph();
            TextView.Blocks.Add(p);

            Brush brush;

            foreach (var token in lexer.Tokenize(value))
            {
                if (ColorDict.TryGetValue(token.Type, out brush))
                {
                    if (builder.Length > 0)
                    {
                        p.Inlines.Add(new Run { Text = builder.ToString() });
                        builder.Clear();
                    }

                    p.Inlines.Add(new Run
                    {
                        Text = value.Substring(token.StartIndex, token.Length),
                        Foreground = brush
                    });
                }
                else
                    builder.Append(value.Substring(token.StartIndex, token.Length));
            }

            if (builder.Length > 0)
            {
                p.Inlines.Add(new Run { Text = builder.ToString() });
                builder.Clear();
            }
        }

        #endregion
    }
}