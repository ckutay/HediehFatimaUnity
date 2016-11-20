using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts
{
	public class FaceController : MonoBehaviour
	{
		[SerializeField]
		private SkinnedMeshRenderer _faceMeshRenderer = null;

		[Space]
		[SerializeField]
		private FacialExpression[] _facialExpressions;

		[SerializeField]
		[Range(0.0001f,1f)]
		private float _blendSmoothDamping = 0.1f;

		[Serializable]
		private class FacialExpression
		{
			[SerializeField]
			private string _expressionName;
			[SerializeField]
			private SkinBlend[] _blends;

			public string ExpressionName {
				get { return _expressionName; }
			}

			public void SetBlends(SkinnedMeshRenderer mesh, float amount)
			{
				foreach (var b in _blends)
					mesh.SetBlendShapeWeight(b.BlendShapeId, b.Weight * amount);
			}
		}

		[Serializable]
		private class SkinBlend
		{
			public int BlendShapeId = 0;

			[Range(0,100)]
			public float Weight = 50f;
		}


		public int NumOfFacialExpressions {
			get { return _facialExpressions.Length; }
		}

		private Dictionary<string,IExpressionController> _instatiatedControllers = new Dictionary<string, IExpressionController>();

		public IExpressionController GetExpressionController(string name)
		{
			IExpressionController c;
			if (_instatiatedControllers.TryGetValue(name, out c))
				return c;

			var facial = _facialExpressions.FirstOrDefault(e => e.ExpressionName == name);
			if (facial == null)
				throw new Exception(string.Format("No facial expression found for \"{0}\"", name));

			c = new FacialExpressionController(this,facial, _faceMeshRenderer);
			_instatiatedControllers.Add(name,c);
			return c;
		}

		public IExpressionController GetExpressionController(int index)
		{
			return GetExpressionController(_facialExpressions[index].ExpressionName);
		}

		public interface IExpressionController
		{
			float Amount { get; set; }
			float TargetAmount { get; set; }
		}

		private class FacialExpressionController : IExpressionController
		{
			private FacialExpression _expression;
			private SkinnedMeshRenderer _mesh;
			private FaceController _parent;

			private float _amount;
			private float _targetAmount;
			private Coroutine _transitionHandle = null;

			public FacialExpressionController(FaceController parent, FacialExpression exp, SkinnedMeshRenderer mesh)
			{
				_parent = parent;
				_expression = exp;
				_mesh = mesh;

				_targetAmount = _amount = 0;
				_expression.SetBlends(_mesh, 0);

			}

			public float Amount {
				get { return _amount; }
				set
				{
					if (_transitionHandle != null)
					{
						_parent.StopCoroutine(_transitionHandle);
						_transitionHandle = null;
					}

					_amount = value;
					_expression.SetBlends(_mesh, _amount);
				}
			}

			public float TargetAmount {
				get { return _targetAmount; }
				set
				{
					_targetAmount = value;
					if (_transitionHandle == null)
						_transitionHandle = _parent.StartCoroutine(AnimationCoroutine());
				}
			}

			private IEnumerator AnimationCoroutine()
			{
				float speed = 0;
				while (Mathf.Abs(_targetAmount - _amount)>0.001f)
				{
					yield return null;
					_amount = Mathf.SmoothDamp(_amount, _targetAmount, ref speed, _parent._blendSmoothDamping, float.MaxValue, Time.smoothDeltaTime);
					_expression.SetBlends(_mesh, _amount);
				}

				_amount = _targetAmount;
				_expression.SetBlends(_mesh, _amount);
				_transitionHandle = null;
			}
		}
	}
}
