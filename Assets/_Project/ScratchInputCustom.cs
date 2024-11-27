using ScratchCardAsset;
using UnityEngine;

public class ScratchInputCustom : MonoBehaviour
{

    #region Events
    public event ScratchHandler OnScratch;
    public event ScratchStartHandler OnScratchStart;
    public event ScratchLineHandler OnScratchLine;
    public event ScratchHoleHandler OnScratchHole;
    public delegate Vector2 ScratchHandler(Vector2 position);
    public delegate void ScratchStartHandler();
    public delegate void ScratchLineHandler(Vector2 start, Vector2 end);
    public delegate void ScratchHoleHandler(Vector2 position);
    #endregion
    [SerializeField] ScratchCard scratchCard;

    public bool IsScratching
    {
        get
        {
            return isScratching;
        }
    }

    private Vector2 eraseStartPositions;
    private Vector2 eraseEndPositions;
    private Vector2 erasePosition;
    private bool isScratching;
    private bool isStartPosition;
#if UNITY_WEBGL
	private bool isWebgl = true;
#else
    private bool isWebgl = false;
#endif

    private const int MaxTouchCount = 10;

    public void Init(ScratchCard card)
    {
        scratchCard = card;
        isScratching = false;
        isStartPosition = true;
        eraseStartPositions = new Vector2();
        eraseEndPositions = new Vector2();
    }
    public void StartScratch()
    {
        isScratching = false;
        isStartPosition = true;
    }
    public void UpdateScratch(Vector3 pos)
    {
        TryScratch(pos);
    }
    public void EndScratch()
    {
        isScratching = false;
    }

    // public void UpdatePos(Vector3 pos)
    // {
    //     if (!scratchCard.InputEnabled)
    //         return;
    //
    //     if (Input.touchSupported && Input.touchCount > 0 && !isWebgl)
    //     {
    //         foreach (var touch in Input.touches)
    //         {
    //             var fingerId = touch.fingerId + 1;
    //             if (touch.phase == TouchPhase.Began)
    //             {
    //                 isScratching = false;
    //                 isStartPosition = true;
    //             }
    //             if (touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary)
    //             {
    //                 TryScratch(touch.position);
    //             }
    //             if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
    //             {
    //                 isScratching = false;
    //             }
    //         }
    //     }
    //     // else
    //     // {
    //     // 	if (Input.GetMouseButtonDown(0))
    //     // 	{
    //     // 		isScratching[0] = false;
    //     // 		isStartPosition[0] = true;
    //     // 	}
    //     // 	if (Input.GetMouseButton(0))
    //     // 	{
    //     // 		TryScratch(0, Input.mousePosition);
    //     // 	}
    //     // 	if (Input.GetMouseButtonUp(0))
    //     // 	{
    //     // 		isScratching[0] = false;
    //     // 	}
    //     // }
    // }

    private void TryScratch(Vector2 position)
    {
        if (OnScratch != null)
        {
            erasePosition = OnScratch(position);
        }

        if (isStartPosition)
        {
            eraseStartPositions = erasePosition;
            eraseEndPositions = eraseStartPositions;
            isStartPosition = !isStartPosition;
        }
        else
        {
            eraseStartPositions = eraseEndPositions;
            eraseEndPositions = erasePosition;
        }

        if (!isScratching)
        {
            eraseEndPositions = eraseStartPositions;
            isScratching = true;
        }
    }

    public void Scratch()
    {

        if (isScratching)
        {
            if (OnScratchStart != null)
            {
                OnScratchStart();
            }

            if (eraseStartPositions == eraseEndPositions)
            {
                if (OnScratchHole != null)
                {
                    OnScratchHole(erasePosition);
                }
            }
            else
            {
                if (OnScratchLine != null)
                {
                    OnScratchLine(eraseStartPositions, eraseEndPositions);
                }
            }
        }
    }
}
