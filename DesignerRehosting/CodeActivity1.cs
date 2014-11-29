using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Activities;
using System.Windows;

namespace DesignerRehosting
{
    [Designer(typeof(ActivityDesigner1))]
    public sealed class CodeActivity1 : CodeActivity
    {
        // 文字列型のアクティビティ入力引数を定義します
        public InArgument<string> Text { get; set; }

        // アクティビティが値を返す場合は、CodeActivity<TResult> から派生して、
        // Execute メソッドから値を返します。
        protected override void Execute(CodeActivityContext context)
        {
            // テキスト型の入力引数のランタイム値を取得します
            string text = context.GetValue(this.Text);


            MessageBox.Show(text);
        }
    }
}
