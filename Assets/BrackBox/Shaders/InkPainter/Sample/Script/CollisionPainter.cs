using UnityEngine;

namespace Es.InkPainter.Sample
{
	[RequireComponent(typeof(Collider), typeof(MeshRenderer))]
	public class CollisionPainter : MonoBehaviour
	{
		[SerializeField]
		private Brush brush = null;

		[SerializeField]
		private int wait = 3;

		private int waitCount;

		public void Awake()
		{
			GetComponent<MeshRenderer>().material.color = brush.Color;
		}

		public void FixedUpdate()
		{
			++waitCount;
		}

		public void OnTriggerStay(Collider collision)
		{
			if(waitCount < wait)
				return;
			waitCount = 0;


            collision.ClosestPointOnBounds(this.transform.position);
            var canvas = collision.GetComponent<InkCanvas>();
            if (canvas != null)
                canvas.Paint(brush, collision.ClosestPointOnBounds(this.transform.position));

   //         foreach (var p in collision.contacts)
			//{
			//	var canvas = p.otherCollider.GetComponent<InkCanvas>();
			//	if(canvas != null)
			//		canvas.Paint(brush, p.point);
			//}
		}
	}
}