
using UnityEngine;

namespace InControl.NativeProfile
{
	public class KeyboardProfile : NativeInputDeviceProfile
	{
		public KeyboardProfile()
		{
			Name = "Keyboard controller";
			Meta = "Xbox One Controller on Windows";
			// Link = "http://www.amazon.com/Microsoft-Xbox-Controller-Cable-Windows/dp/B00O65I2VY";

			DeviceClass = InputDeviceClass.Controller;
			DeviceStyle = InputDeviceStyle.XboxOne;

			IncludePlatforms = new[] {
				"Windows"
			};

			ExcludePlatforms = new[] {
				"Windows 7",
				"Windows 8"
			};

			MaxSystemBuildNumber = 14392;

			ButtonMappings = new[]
            {
                new InputControlMapping
                {
                    Handle = "Jump",
                    Target = InputControlType.Action1,
                    Source = new UnityKeyCodeSource(KeyCode.Space)
                },
                new InputControlMapping
                {
                    Target = InputControlType.Action4,
                    Source = new UnityKeyCodeSource(KeyCode.E)
                },
                new InputControlMapping
                {
                    Target = InputControlType.Action3,
                    Source = new UnityKeyCodeSource(KeyCode.R)
                },
            };

            AnalogMappings = new[]
            {
                new InputControlMapping
                {
                    Handle = "Horizontal",
                    Target = InputControlType.LeftStickX,
                    Source = new UnityKeyCodeAxisSource(KeyCode.A, KeyCode.D)
                },
                new InputControlMapping
                {
                    Handle = "Vertical",
                    Target = InputControlType.LeftStickY,
                    Source = new UnityKeyCodeAxisSource(KeyCode.S, KeyCode.W)
                },
            };
		}
	}
	// @endcond
}