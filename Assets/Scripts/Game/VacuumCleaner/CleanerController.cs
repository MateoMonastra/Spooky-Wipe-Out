using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using VacuumCleaner.Modes;

//Patron State
namespace VacuumCleaner
{
    public class CleanerController: MonoBehaviour
    {
        [SerializeField] private List<ToolByID> toolsByIDs = new List<ToolByID>();
        [SerializeField] private float weaponDelay;
        private WaitForSeconds _waitForSeconds;
        private ITool _currentTool;
        

        private void Start()
        {
            SwitchToTool(0);
            _waitForSeconds = new WaitForSeconds(weaponDelay);
        }
        
        public IEnumerator SwitchToTool(int id)
        {
            
            var toolById = toolsByIDs.Where(toolById=> toolById.Id == id).FirstOrDefault();

            if(toolById == null)
            {
                Debug.LogError($"No tool for the id {id}");
                yield break;
            }

            var nextTool = toolById.tool.GetTool();

            if(nextTool == null || nextTool == _currentTool)
            {
                yield break;
            }

            _currentTool?.PowerOff();

            _currentTool = nextTool;

            yield return _waitForSeconds;
            
            _currentTool?.PowerOn();
        }
    }
}
