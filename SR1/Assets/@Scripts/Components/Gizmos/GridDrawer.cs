using UnityEditor;
using UnityEngine;
using static Define;

public class GridDrawer : MonoBehaviour
{
#if UNITY_EDITOR
    void OnDrawGizmos()
    {
        DrawPlayerRange();
        DrawMonsterRange();
        DrawPositions();
    }

    void DrawPositions()
    {
        Vector3Int campPos = Managers.Map.World2Cell(Managers.Object.HeroCamp.Position);

        int basicDetectionRange;
        int extraVerticalDetectionRange;


        //To Dragon.
        //캠프시 true, 평소에 false입니다.
        //이 설정에서는 카메라 사이즈 30으로 하면 될듯합니다.
        bool onIdle = Managers.Object.HeroCamp.CampState == ECampState.CampMode;

        if(onIdle)
        {
            basicDetectionRange = BASIC_DETECTION_RANGE_ON_IDLE;
            extraVerticalDetectionRange = EXTRA_VERTICAL_DETECTION_RANGE_ON_IDLE;
        }
        else
        {
            basicDetectionRange = BASIC_DETECTION_RANGE;
            extraVerticalDetectionRange = EXTRA_VERTICAL_DETECTION_RANGE;
        }

        Vector3Int BottomLeft = new Vector3Int(campPos.x - basicDetectionRange - extraVerticalDetectionRange, campPos.y - extraVerticalDetectionRange);
        Vector3Int BottomRight = new Vector3Int(campPos.x - extraVerticalDetectionRange, campPos.y - basicDetectionRange - extraVerticalDetectionRange);
        Vector3Int TopRight = new Vector3Int(campPos.x + basicDetectionRange + extraVerticalDetectionRange, campPos.y + extraVerticalDetectionRange);
        Vector3Int TopLeft = new Vector3Int(campPos.x + extraVerticalDetectionRange, campPos.y + basicDetectionRange + extraVerticalDetectionRange);

        Vector3Int pos;

        bool drawing = true;
        while (drawing)
        {
            pos = BottomLeft;

            while (pos != BottomRight)
            {
                pos += new Vector3Int(1, -1);
                DrawGizmo(pos);
                if(drawing)
                {
                    if (pos == campPos)
                        drawing = false;
                }    
            }
            if (drawing == false)
                return;

            while (pos != TopRight)
            {
                pos += new Vector3Int(1, 1);
                DrawGizmo(pos);

                if (drawing)
                {
                    if (pos == campPos)
                        drawing = false;
                }
            }
            if (drawing == false)
                return;
            while (pos != TopLeft)
            {
                pos += new Vector3Int(-1, 1);
                DrawGizmo(pos);

                if (drawing)
                {
                    if (pos == campPos)
                        drawing = false;
                }
            }
            if (drawing == false)
                return;
            while (pos != BottomLeft)
            {
                pos += new Vector3Int(-1, -1);
                DrawGizmo(pos);

                if (drawing)
                {
                    if (pos == campPos)
                        drawing = false;
                }
            }
            BottomLeft += Vector3Int.right;
            BottomRight += Vector3Int.up;
            TopRight += Vector3Int.left;
            TopLeft += Vector3Int.down;
        }
    }

    void DrawGizmo(Vector3Int tilePos)
    {
        Vector3 centerWorldPos = Managers.Object.HeroCamp.Position;
        Vector3 worldPos = Managers.Map.Cell2World(tilePos);


        if (Managers.Map._cells.TryGetValue(tilePos, out BaseObject obj))
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawSphere(worldPos, 0.15f);
            Handles.Label(worldPos + Vector3.down * 0.25f, $"({tilePos.x}, {tilePos.y})", GUI.skin.box);
        }
        else
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(worldPos, 0.15f);
            Handles.Label(worldPos + Vector3.down * 0.25f, $"({tilePos.x}, {tilePos.y})", GUI.skin.box);
        }

    }

    void DrawPlayerRange()
    {
        // Vector3 centerPos = Managers.Object.HeroCamp.Position;
        // Gizmos.DrawWireSphere(centerPos, 10f );
        // DrawRange(centerPos, SCAN_RANGE.x,SCAN_RANGE.y, Color.red);
        // Handles.Label(centerPos, $"Camp", GUI.skin.box);
    }

    void DrawMonsterRange()
    {
        // Vector3 centerPos = Managers.Object.Camp.GetPosition();
        // DrawRange(Define.EObjectType.Monster, centerPos, Define.MONSTER_ATTACK_RANGE , Define.MONSTER_ATTACK_RANGE + 2, Color.red);
    }
    
    void DrawRange(Vector3 centerPos, float rangeX, float rangeY, Color color)
    {

    }
#endif

}
