namespace VP.Notifications.Toast
{
    /// <summary>
    /// 通知需要使用到的常量
    /// </summary>
    public class ToastConst
    {
        public const string Id = "id";
        public const string Group = "group";
        public const string DefaultGroup = "defaultGroup";


        public class Action
        {
            public const string Key = "action";
            public abstract class KeyValue { }
            public const string Value = "actionValue";
        }

        public class ProgressBar
        {
            /// <summary>
            /// 进度不确定值
            /// </summary>
            public const string ValueIndeterminate = "indeterminate";

            /// <summary>
            /// 进度值 取值0-1 double
            /// </summary>
            public const string Value = "progressBarValue";

            /// <summary>
            /// 进度状态
            /// </summary>
            public const string Status = "progressBarStatus";

            /// <summary>
            /// 进度标题
            /// </summary>
            public const string Title = "progressBarTitle";

            /// <summary>
            /// 进度值字符串
            /// </summary>
            public const string ValueString = "progressBarValueString";
        }

    }
}
