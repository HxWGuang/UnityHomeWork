#if UNITY_EDITOR
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.AI;

public class DrawNavMesh : MonoBehaviour
{
    // x坐标格子的数量
    public int width;
    // y坐标格子的数量
    public int height;
    // 格子的大小
    public int size;

    // 范围
    public float range;

    private void OnDrawGizmosSelected()
    {
        if (NavMesh.CalculateTriangulation().indices.Length > 0)
        {
            var scenePath = UnityEditor.SceneManagement.EditorSceneManager.GetSceneAt(0).path;
            var sceneName = System.IO.Path.GetFileName(scenePath);
            var filePath = Path.ChangeExtension(Path.Combine(Application.dataPath, sceneName), "txt");
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }

            var sb = new StringBuilder();
            sb.AppendFormat("scene={0}", sceneName).AppendLine();
            sb.AppendFormat("width={0}", width).AppendLine();
            sb.AppendFormat("height={0}", height).AppendLine();
            sb.AppendFormat("size={0}", size).AppendLine();
            sb.Append("data={").AppendLine();
            
            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(transform.position, 1);

            var widthHalf = (float) width / 2f;
            var heightHalf = (float) height / 2f;
            var sizeHalf = (float) size / 2f;
            
            //从左到右，从下到上，一次性写入每个格子的数据
            for (int i = 0; i < height; i++)
            {
                sb.Append("\t{");
                var startPos = new Vector3(-widthHalf + sizeHalf, 0, -heightHalf + (i * size) + sizeHalf);
                for (int j = 0; j < width; j++)
                {
                    var source = startPos + Vector3.right * size * j;
                    var color = Color.red;
                    int a = 0;
                    // 检测当前格子是否可以走
                    if (NavMesh.SamplePosition(source, out NavMeshHit hit, range, NavMesh.AllAreas))
                    {
                        color = Color.blue;
                        a = 1;
                    }

                    sb.AppendFormat(j > 0 ? ",{0}" : "{0}", a);
                    Debug.DrawRay(source, Vector3.up, color);
                }

                sb.Append("}").AppendLine();
            }
            sb.Append("}").AppendLine();
            // 绘制格子的总区域
            Gizmos.DrawLine(new Vector3(-widthHalf, 0, -heightHalf), new Vector3(widthHalf, 0, -heightHalf));
            Gizmos.DrawLine(new Vector3(widthHalf, 0, -heightHalf), new Vector3(widthHalf, 0, heightHalf));
            Gizmos.DrawLine(new Vector3(widthHalf, 0, heightHalf), new Vector3(-widthHalf, 0, heightHalf));
            Gizmos.DrawLine(new Vector3(-widthHalf, 0, heightHalf), new Vector3(-widthHalf, 0, -heightHalf));
            
            // 写入文件
            File.WriteAllText(filePath, sb.ToString());
        }
    }
}
#endif
