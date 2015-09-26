using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace linux_du {
    class Program {
        private String dataFile = "";
        private long size = 1;
        private String baseDir = Path.Combine(Directory.GetCurrentDirectory(), "out");
        private String dummyName = "dummy";
        static void Main(string[] args) {
            new Program(args).run();
        }
        public Program(string[] args) {
            if (args.Length == 1) {
                this.dataFile = args[0];
                return;
            }
            for (int i = 0; i < args.Length; i++) {
                var item = args[i].Trim();
                if (item == "-dir" && i + 1 < args.Length) {
                    this.baseDir = System.IO.Path.GetFullPath(System.IO.Path.Combine(this.baseDir, args[i + 1]));
                    i++;
                } else if (item == "-data" && i + 1 < args.Length) {
                    this.dataFile = System.IO.Path.GetFullPath(System.IO.Path.Combine(this.dataFile, args[i + 1]));
                    i++;
                } else if (item == "-dummy" && i + 1 < args.Length) {
                    this.dummyName = args[i + 1];
                    i++;
                }
            }
        }
        public void run() {
            //引数ファイルの解析
            List<Data> datas = new List<Data>();
            foreach (var item in File.ReadLines(this.dataFile)) {
                datas.Add(new Data(item.Trim()));
            }
            // ファイルを順番に解析していく
            while (0 < datas.Count) {
                int index = -1;
                //一番深いサブディレクトリを探す
                for (int i = 0; i < datas.Count; i++) {
                    if (!hasSubDirectory(datas, datas[i].path)) {
                        index = i;
                        break;
                    }
                }
                if (index != -1) {
                    //一番深いサブディレクトリの中に、ダミーデータを作る
                    createDummy(datas[index].path, datas[index].size);
                    //ダミーデータを作ったサブディレクトリの親ディレクトリから、ダミーデータの分のサイズを引く
                    minusSize(ref datas, datas[index].path, datas[index].size);
                    //一番深いサブディレクトリのデータを消す
                    datas.RemoveAt(index);
                }
            }
        }
        private void minusSize(ref List<Data> datas, String baseDir, long minusSize) {
            foreach (var item in datas) {
                if (baseDir.StartsWith(item.path) || item.path == "") {
                    item.size -= minusSize;
                }
            }
        }

        private Boolean hasSubDirectory(List<Data> datas, String path) {
            foreach (var item in datas) {
                if (item.path == path) {
                    continue;
                } else if (item.path.StartsWith(path + "/")) {
                    return true;
                }
            }
            return false;
        }
        private void createDummy(String directory, long size) {
            if (size <= 0) {
                return;
            }
            if (directory == "") {
                directory = ".";
            }
            String createDirectoryPath = System.IO.Path.Combine(this.baseDir, directory.TrimStart('/').Replace('/', '\\')); ;
            String createFilePath = System.IO.Path.Combine(this.baseDir, directory.TrimStart('/').Replace('/', '\\') + @"\" + this.dummyName);
            System.IO.Directory.CreateDirectory(createDirectoryPath);
            using (var fs = new FileStream(createFilePath, FileMode.Create, FileAccess.Write, FileShare.None)) {
                fs.SetLength(size);
            }
        }
    }
    public class Data {
        public long size;
        public String path;
        private Regex pat = new Regex(@"^(\d+)\s*(.+)");
        public Data(String line) {
            var match = pat.Match(line);
            if (match.Success) {
                this.size = long.Parse(match.Groups[1].Value);
                this.path = match.Groups[2].Value.Trim().TrimStart('.');
            } else {
                throw new ArgumentException();
            }
        }
        public override string ToString() {
            return $"{size} {path}";
        }
    }
}
