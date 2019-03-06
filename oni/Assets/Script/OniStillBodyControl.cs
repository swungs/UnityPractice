using UnityEngine;
using System.Collections;

public class OniStillBodyControl : MonoBehaviour {


	public OniEmitterControl	emitter_control = null;

	void 	Start()
	{
	}
	
	void 	Update()
	{
	}

	void	OnCollisionEnter(Collision other)
	{
		if(other.gameObject.tag == "OniYama") {

			// 도깨비 무리에 부딪혔을 때 소리를 재생한다.
			this.emitter_control.PlayHitSound();

			// 직접 SE를 재생하면 짧은 간격으로 소리가 재생되기 때문에 소리가 겹치게 되어 제대로 들리지 않는다. 
			// OniEmitterControl 으로 적절한 간격으로 소리가 재생되도록 한다.
			// 
		}

		if(other.gameObject.tag == "Floor") {

			// 물리 계산을 멈추기 위해 rigidbody 컴포넌트를 삭제한다.
			// 무리일 수 있으나 Sleep() 의 경우 서서히 작동할 수 있다.
			Destroy(this.GetComponent<Rigidbody>());
		}
	}
}
