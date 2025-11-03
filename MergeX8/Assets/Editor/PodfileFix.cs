using System.IO;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

public class PostProcessIOS : MonoBehaviour
{
    [PostProcessBuild(49)] //must be between 40 and 50 to ensure that it's not overriden by Podfile generation (40) and that it's added before "pod install" (50)
    private static void PostProcessBuild_iOS(BuildTarget target, string buildPath)
    {
        if (target == BuildTarget.iOS)
        {
            using (StreamWriter sw = File.AppendText(buildPath + "/Podfile"))
            {
                //in this example I'm adding an app extension
                sw.WriteLine("post_install do |installer|");
                sw.WriteLine("  installer.pods_project.targets.each do |target|");
                sw.WriteLine("    target.build_configurations.each do |config|");
                sw.WriteLine("      if target.respond_to?(:product_type) and target.product_type == \"com.apple.product-type.bundle\"");
                sw.WriteLine("        target.build_configurations.each do |config|");
                sw.WriteLine("          config.build_settings['DEVELOPMENT_TEAM'] = \"RCHH38BABG\"");
                sw.WriteLine("        end");
                sw.WriteLine("      end");
                sw.WriteLine("    end");
                sw.WriteLine("  end");
                sw.WriteLine("end");
            }
        }
    }
}