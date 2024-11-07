using UnityEngine;
using UnityEngine.UI;

public class CleanerSelectionUIControler : MonoBehaviour
{
    private static CleanerSelectionUIControler _instance;

    [Header("Clean UI Elements")] 
    [SerializeField] private Sprite powerOnVacuum;
    [SerializeField] private Sprite powerOffVacuum;
    [SerializeField] private Sprite powerOnWashFloor;
    [SerializeField] private Sprite powerOffWashFloor;

    [Header("HUD Elements")] 
    [SerializeField] private GameObject vacuumHUD;
    [SerializeField] private GameObject washFloorHUD;
    
    [Header("Player Elements")] 
    [SerializeField] private GameObject vacuumNuzzle;
    [SerializeField] private GameObject washFloorNuzzle;

    private Image _currentVacuum;
    private Image _currentWashFloor;

    public static CleanerSelectionUIControler GetInstance()
    {
        if (_instance == null)
        {
            _instance = FindObjectOfType<CleanerSelectionUIControler>();
        }

        return _instance;
    }

    private void Awake()
    {
        // Singleton pattern implementation in Awake
        if (_instance == null)
        {
            _instance = this;
        }
        else if (_instance != this)
        {
            Destroy(gameObject); // Destroy duplicates if any exist
        }
    }
    
    private void OnDestroy()
    {
        _instance = null;
    }

    private void Start()
    {
        // Get Image components from HUD objects
        if (vacuumHUD != null)
        {
            _currentVacuum = vacuumHUD.GetComponent<Image>();
            if (_currentVacuum == null)
            {
                Debug.LogWarning($"{name}: vacuumHUD does not have an Image component.");
            }
        }
        else
        {
            Debug.LogWarning($"{name}: vacuumHUD is not assigned in the Inspector.");
        }

        if (washFloorHUD != null)
        {
            _currentWashFloor = washFloorHUD.GetComponent<Image>();
            if (_currentWashFloor == null)
            {
                Debug.LogWarning($"{name}: washFloorHUD does not have an Image component.");
            }
        }
        else
        {
            Debug.LogWarning($"{name}: washFloorHUD is not assigned in the Inspector.");
        }
        
        PowerOnVacuum();
    }

    public void PowerOnVacuum()
    {
        if (_currentVacuum == null || _currentWashFloor == null)
        {
            Debug.LogWarning("Cannot change sprites because an Image reference is missing.");
            return;
        }
        _currentVacuum.sprite = powerOnVacuum;
        _currentWashFloor.sprite = powerOffWashFloor;
        
        vacuumNuzzle.SetActive(true);
        washFloorNuzzle.SetActive(false);
    }

    public void PowerOnWashFloor()
    {
        if (_currentWashFloor == null || _currentVacuum == null)
        {
            Debug.LogWarning("Cannot change sprites because an Image reference is missing.");
            return;
        }
        _currentWashFloor.sprite = powerOnWashFloor;
        _currentVacuum.sprite = powerOffVacuum;
        
        vacuumNuzzle.SetActive(false);
        washFloorNuzzle.SetActive(true);
    }
}