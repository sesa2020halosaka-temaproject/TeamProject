//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using Cinemachine;

//namespace TeamProject
//{
//    public class FixedDollyCamera : MonoBehaviour
//{
//    [SerializeField] private CinemachineVirtualCamera virtualCamera;
//    //[SerializeField] private float cycleTime = 10.0f;

//    private CinemachineTrackedDolly dolly;
//    public float pathPositionMax;
//    public float pathPositionMin;

//        public DollyCamera _DollyCam;
//        public GameObject _DollyCam_Obj;
//    //public float AddTime;//移動速度の方向
//    //public float MoveRatio;//移動速度の倍率

//    //カメラのパスの位置の移動
//    //public enum DOLLY_MOVE
//    //{
//    //    FIXING,//固定
//    //    GO,  //進める
//    //    BACK,   //戻る
//    //    ALL_STATES//全要素数

//    //}
//    //public DOLLY_MOVE m_DollyMove;

//    //public bool Move_flag;//カメラが移動しているかどうか
//    //public CinemachinePathBase m_Dolly_GO;
//    //public CinemachinePathBase m_Dolly_BACK;
//    public WayPoint_Box m_WP;//ドリールートのパス位置格納用
//    private void Awake()
//    {
//        // バーチャルカメラがセットされていなければ中止
//        if (this.virtualCamera == null)
//        {
//            this.enabled = false;
//            Debug.Log("バーチャルカメラが" + this.virtualCamera);
//            return;
//        }

//        // ドリーコンポーネントを取得できなければ中止
//        this.dolly = this.virtualCamera.GetCinemachineComponent<CinemachineTrackedDolly>();
//        if (this.dolly == null)
//        {
//            Debug.Log("Dollyコンポーネントが" + this.dolly);

//            this.enabled = false;
//            return;

//        }
//    }
//    private void Start()
//    {
//        m_WP = GameObject.Find("WayPoint_Box").GetComponent<WayPoint_Box>();
//        Debug.Log(dolly.name + "：m_Path：" + dolly.m_Path);

//        // Positionの単位をトラック上のウェイポイント番号基準にするよう設定
//        this.dolly.m_PositionUnits = CinemachinePathBase.PositionUnits.PathUnits;


//       // this.dolly.m_Path = m_Dolly_GO;//前進用ドリーパスをセット

//        // ウェイポイントの最大番号・最小番号を取得
//        this.pathPositionMax = this.dolly.m_Path.MaxPos;
//        this.pathPositionMin = this.dolly.m_Path.MinPos;

//        //this.DollyState("ZERO");

//    }

//    // Update is called once per frame
//    void Update()
//    {

//           // this.transform.position = _DollyCam_Obj.transform.position;
//    }

//        public void LookAtTargetChange(GameObject NextTarget)
//        {
//            virtualCamera.LookAt = NextTarget.transform;
//        }
//        public void SetPosition(GameObject _MainDolly)
//        {
//            this.transform.position = _MainDolly.transform.position;
//        }
//        public void SetCinemachineTrackedDolly(CinemachineTrackedDolly _MainDolly)
//        {
//            dolly.m_PathPosition = _MainDolly.m_PathPosition;
//            dolly.m_Path = _MainDolly.m_Path;
//        }

//    }//public class FixedDollyCamera : MonoBehaviour END
//}//namespace END