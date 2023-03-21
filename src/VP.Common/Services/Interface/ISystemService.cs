namespace VP.Common.Services.Interface
{
    /// <summary>
    /// 系统信息相关服务
    /// </summary>
    public interface ISystemService
    {
        //todo 注释
        /// <summary>
        /// 1
        /// </summary>
        /// <returns></returns>
        public DateTime GetSystemInstallTime();

        public string GetUUID();
    }
}