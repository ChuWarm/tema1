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
            enabled = false;  // 컴포넌트 비활성화
            return;
        }

        // 애니메이터 컨트롤러 확인
        if (_animator.runtimeAnimatorController == null)
        {
            enabled = false;
            return;
        }
        
        bool hasOpenParam = false;
        bool hasCloseParam = false;
        foreach (AnimatorControllerParameter param in _animator.parameters)
        {
            if (param.nameHash == OpenHash) hasOpenParam = true;
            if (param.nameHash == CloseHash) hasCloseParam = true;
        }

        if (!hasOpenParam || !hasCloseParam)
        {
            enabled = false;
            return;
        }
    }

    public void Open()
    {
        string doorName = gameObject.name;
        Debug.LogWarning($"[{doorName}] Open() CALLED. IsEnabled: {enabled}, Current _isOpen: {this._isOpen}, Current _isAnimating: {this._isAnimating}. Stack: {System.Environment.StackTrace}");

        if (!enabled)
        {
            Debug.LogWarning($"[{doorName}] Open() 중단: Door component is not enabled.");
            return;
        }
        if (_isOpen)
        {
            Debug.LogWarning($"[{doorName}] Open() 중단: Door is already open (_isOpen = true).");
            return;
        }
        if (_isAnimating)
        {
            Debug.LogWarning($"[{doorName}] Open() 중단: Door is already animating (_isAnimating = true).");
            return;
        }

        if (_animator != null)
        {
            _isAnimating = true;
            _animator.SetTrigger(OpenHash);
        }
    }

    public void Close()
    {
        string doorName = gameObject.name;
            _isAnimating = true;
            _animator.SetTrigger(CloseHash);
    }

    // 애니메이션 이벤트로 호출될 메서드들
    public void OnOpenAnimationComplete()
    {
        string doorName = gameObject.name;
        this._isOpen = true;
        this._isAnimating = false;
    }

    public void OnCloseAnimationComplete()
    {
        string doorName = gameObject.name;
        this._isOpen = false;
        this._isAnimating = false;
    }

    public bool IsOpen => _isOpen;
    public bool IsAnimating => _isAnimating;
}
