//-----------------------------------------------------------------------
// <copyright file="PatternList.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation 2010. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

/****************************************************************************
 * PatternList.cs
 * 
 *   This file contains methods to fire an event on specified pattern.
 * 
******************************************************************************/
using System;
using System.Windows.Automation;
using System.Windows.Forms;
using System.Windows;

namespace SharingService.Web.Automation.Util
{
    /// <summary>
    /// Class which has methods for all Patterns
    /// </summary>
    public static class PatternList
    {
        /// <summary>
        /// Element which is the parent of the control on which action takes place
        /// </summary>
        private static AutomationElement parentElement;

        /// <summary>
        /// Control on which action takes place
        /// </summary>
        private static AutomationElement childElement;

        /// <summary>
        /// Variable holds the result of the step/Test case
        /// </summary>
        private static bool result;

        /// <summary>
        /// Gets or sets ChildElement
        /// </summary>
        public static AutomationElement ChildElement
        {
            get
            {
                return childElement;
            }
            set
            {
                childElement = value;
            }
        }

        /// <summary>
        /// Gets or sets ParentElement
        /// </summary>
        public static AutomationElement ParentElement
        {
            get
            {
                return parentElement;
            }
            set
            {
                parentElement = value;
            }
        }

        /// <summary>
        /// Method performs the Click Action on specified control 
        /// </summary>
        /// <returns>bool(Click Success - TRUE or Failure - FALSE)</returns>
        public static bool DoClick()
        {
            result = false;
            try
            {
                InvokePattern ptnInvoke = childElement.GetCurrentPattern(InvokePattern.Pattern) as InvokePattern;
                System.Threading.Thread.Sleep(5000);
                ptnInvoke.Invoke();
                result = true;
            }
            catch (ElementNotEnabledException ex)
            {
                Console.Write(ex);
                result = false;
            }
            return result;
        }

        /// <summary>
        /// Method performs the Select Action on specified control
        /// </summary>
        /// <returns>True if the action performed successfully</returns>
        public static bool DoSelect()
        {
            result = false;
            try
            {
                SelectionItemPattern selectionItemPattern = childElement.GetCurrentPattern(SelectionItemPattern.Pattern)
                    as SelectionItemPattern;
                System.Threading.Thread.Sleep(5000);
                selectionItemPattern.Select();
                result = true;
            }
            catch (ElementNotEnabledException ex)
            {
                Console.Write(ex);
                result = false;
            }

            return result;
        }

        /// <summary>
        /// Method enters Text on to specified control.
        /// </summary>
        /// <param name="actualValue">Value to be entered</param>
        /// <returns>bool(Sendkeys Success - TRUE or Failure - FALSE)</returns>
        public static bool DoSendKeys(string actualValue)
        {
            result = false;
            try
            {
                SendKeys.SendWait(actualValue);
                System.Threading.Thread.Sleep(2000);
                result = true;
            }
            catch (ElementNotEnabledException ex)
            {
                Console.Write(ex);
                result = false;
            }

            return result;
        }

        /// <summary>
        /// Method enters Text into Textbox using Value pattern 
        /// </summary>
        /// <returns>bool(Enter Text Success - TRUE or Failure - FALSE)</returns>        
        public static bool DoEnterText(string valueToBeEntered)
        {
            result = false;
            try
            {
                if (!childElement.Current.IsEnabled)
                {
                    throw new InvalidOperationException(
                        "The control with an AutomationID of "
                        + childElement.Current.AutomationId.ToString()
                        + " is not enabled.\n\n");
                }

                if (!childElement.Current.IsKeyboardFocusable)
                {
                    throw new InvalidOperationException(
                        "The control with an AutomationID of "
                        + childElement.Current.AutomationId.ToString()
                        + "is read-only.\n\n");
                }

                object valuePattern = null;
                if (!childElement.TryGetCurrentPattern(ValuePattern.Pattern, out valuePattern))
                {
                    childElement.SetFocus();
                    System.Threading.Thread.Sleep(1000);
                    SendKeys.SendWait("^{HOME}");   // Move to start of control
                    SendKeys.SendWait("^+{END}");   // Select everything
                    SendKeys.SendWait("{DEL}");     // Delete selection
                    SendKeys.SendWait(valueToBeEntered);
                    result = true;
                }
                else
                {
                    childElement.SetFocus();
                    ((ValuePattern)valuePattern).SetValue(valueToBeEntered);
                    result = true;
                }
            }
            catch (ElementNotEnabledException ex)
            {
                Console.WriteLine(ex);
                result = false;
            }

            return result;
        }
    }
}
