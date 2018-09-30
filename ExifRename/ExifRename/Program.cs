namespace ExifRename
{
    class Program
    {
        static void Main(string[] args)
        {
            foreach (string aFileName in args)
            {
                try
                {
                    System.IO.FileInfo aFileInfo = new System.IO.FileInfo(aFileName);
                    if (!aFileInfo.Exists) throw new System.ApplicationException("未找到指定文件！");

                    System.Drawing.Image aImage = System.Drawing.Image.FromFile(aFileName);
                    System.Drawing.Imaging.PropertyItem aPropertyItem = aImage.GetPropertyItem(0x9003);
                    aImage.Dispose();
                    
                    string aTime = System.Text.Encoding.ASCII.GetString(aPropertyItem.Value, 0, aPropertyItem.Value.Length-1).Replace(":", "").Replace(' ', '.');
                    string aNewFileName = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(aFileName), string.Format("{0}{1}", aTime, System.IO.Path.GetExtension(aFileName)));
                    if (aFileName != aNewFileName) aFileInfo.MoveTo(aNewFileName);
                }
                catch (System.Exception ex)
                {
                    System.Console.WriteLine(ex.Message);
                }
            }
            System.Console.Write("按回车键结束。。。");
            System.Console.ReadLine();
        }
    }
}
