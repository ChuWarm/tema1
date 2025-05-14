using UnityEngine;

public class Door : MonoBehaviour
{
    private Animator _animator;
    private bool _isOpen = false;
    private bool _isAnimating = false;
    private static readonly int OpenHash = Animator.StringToHash("Open");
    private static readonly int CloseHash = Animator.StringToHash("Close");

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        if (_animator == null)
        {
            Debug.LogError($"Door {gameObject.name}: Animator component is required but not found!");
            enabled = false;  // 컴포넌트 비활성화
            return;
        }

        // 애니메이터 컨트롤러 확인
        if (_animator.runtimeAnimatorController == null)
        {
            Debug.LogError($"Door {gameObject.name}: Animator Controller is not assigned!");
            enabled = false;
            return;
        }

        // 파라미터 존재 여부 확인
        bool hasOpenParam = false;
        bool hasCloseParam = false;
        foreach (AnimatorControllerParameter param in _animator.parameters)
        {
            if (param.nameHash == OpenHash) hasOpenParam = true;
            if (param.nameHash == CloseHash) hasCloseParam = true;
        }

        if (!hasOpenParam || !hasCloseParam)
        {
            Debug.LogError($"Door {gameObject.name}: Required animator parameters 'Open' or 'Close' not found!");
            enabled = false;
            return;
        }
    }

    public void Open()
    {
        if (!enabled || _isOpen || _isAnimating)
        {
            return;
        }

        if (_animator != null)
        {
            _isAnimating = true;
            _animator.SetTrigger(OpenHash);
        }
        else
        {
            _isAnimating = true;
            _animator.SetTrigger(CloseHash);
        }
    }

    public void Close()
    {
        string doorName = gameObject.name;
        if (_animator != null)
        {
            _isAnimating = true;
            _animator.SetTrigger(CloseHash);
        }
        else
        {
            Debug.LogError($"Door {doorName}: Close() 호출되었으나, _animator가 null입니다. Awake에서 초기화 실패 가능성 확인 필요.");
        }
    }

    // 애니메이션 이벤트로 호출될 메서드들
    public void OnOpenAnimationComplete()
    {
        _isOpen = true;
        _isAnimating = false;
    }

    public void OnCloseAnimationComplete()
    {
        _isOpen = false;
        _isAnimating = false;
    }

    public bool IsOpen => _isOpen;
    public bool IsAnimating => _isAnimating;
}
