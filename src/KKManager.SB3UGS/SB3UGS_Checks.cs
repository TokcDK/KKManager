using KKManager.Util;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KKManager.SB3UGS
{
    public class SB3UGS_Checks
    {
        public static bool FileCanBeCompressed(FileInfo x, DirectoryInfo rootDirectory)
        {
            if (!SB3UGS_Utils.FileIsAssetBundle(x)) return false;

            // Files inside StreamingAssets are hash-checked so they can't be changed
            var isStreamingAsset = x.FullName.Substring(rootDirectory.FullName.Length)
                .Split(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar)
                .Contains("StreamingAssets", StringComparer.OrdinalIgnoreCase);
            if (isStreamingAsset)
            {
                Console.WriteLine($"Skipping file {x.FullName} - Files inside StreamingAssets folder are hash-checked and can't be modified.");
                return false;
            }

            return true;
        }

        public static void CompressFiles(IReadOnlyList<FileInfo> files, bool randomizeCab)
        {
            if (!SB3UGS_Initializer.CheckIsAvailable())
            {
                MessageBox.Show(
                    T._("SB3UGS has not been found in KK Manager directory or it failed to be loaded. Reinstall KK Manager and try again."),
                    T._("Compress files"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            LoadingDialog.ShowDialog(null, T._("Compressing asset bundle files"), dialogInterface =>
            {
                dialogInterface.SetMaximum(files.Count);

                var excs = new ConcurrentBag<Exception>();
                long totalSizeSaved = 0;
                var count = 0;

                Parallel.ForEach(files, file =>
                {
                    dialogInterface.SetProgress(count++, T._("Compressing ") + file.Name);

                    try
                    {
                        var origSize = file.Length;
                        SB3UGS_Utils.CompressBundle(file.FullName, randomizeCab);
                        file.Refresh();
                        totalSizeSaved += origSize - file.Length;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(T._($"Failed to compress file {file.FullName} - {ex.Message}"));
                        excs.Add(ex);
                    }
                });

                if (excs.Any())
                    MessageBox.Show(T._($"Successfully compressed {files.Count - excs.Count} out of {files.Count} files, see log for details. Saved {FileSize.FromBytes(totalSizeSaved)}."), T._("Compress files"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                else
                    MessageBox.Show(T._($"Successfully compressed {files.Count} files. Saved {FileSize.FromBytes(totalSizeSaved)}."), T._("Compress files"), MessageBoxButtons.OK, MessageBoxIcon.Information);
            });
        }
    }
}
