using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using UnityEngine;

[RequireComponent (typeof(SpriteRenderer))]
public class DaniTech_SpriteRendererSequencer : MonoBehaviour
{
    [Header("스프라이트 설정")]
    [SerializeField] private Sprite[] SpriteArray_Sprite;

    [Header("재생 설정")]
    [SerializeField] private float _sequenceInterval = 0.1f;

    [SerializeField] private bool _isLoop = true;

    private SpriteRenderer _spriteRenderer;
    private CancellationTokenSource _cancelToken;

    private void Awake()
    {
        _spriteRenderer = this.GetComponent<SpriteRenderer>();
    }

    private void OnEnable()
    {
        PlayAnimation().Forget();
    }

    private void OnDisable()
    {
        StopAnimation();
    }

    private async UniTaskVoid PlayAnimation()
    {
        try
        {
            if (SpriteArray_Sprite == null || SpriteArray_Sprite.Length == 0)
            {
                return;
            }

            if (_spriteRenderer == null)
            {
                return;
            }

            if (_sequenceInterval <= 0f)
            {
                _sequenceInterval = 0.1f;
            }

            StopAnimation();

            _cancelToken = new CancellationTokenSource();

            int currentIndex = 0;

            while (true)
            {
                Sprite currentSprite = SpriteArray_Sprite[currentIndex];

                if (currentSprite != null)
                {
                    _spriteRenderer.sprite = currentSprite;
                }

                await UniTask.Delay(
                    TimeSpan.FromSeconds(_sequenceInterval),
                    cancellationToken: _cancelToken.Token
                );

                currentIndex++;

                if (currentIndex >= SpriteArray_Sprite.Length)
                {
                    if (_isLoop == true)
                    {
                        currentIndex = 0;
                    }
                    else
                    {
                        break;
                    }
                }
            }
        }
        catch (OperationCanceledException)
        {
            // 오브젝트가 꺼지거나 삭제될 때 애니메이션이 멈추는 정상 흐름
        }
    }

    private void StopAnimation()
    {
        if (_cancelToken != null)
        {
            _cancelToken.Cancel();
            _cancelToken.Dispose();
            _cancelToken = null;
        }
    }

    private void OnDestroy()
    {
        StopAnimation();
    }

}
