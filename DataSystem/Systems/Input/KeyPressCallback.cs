namespace CMDR.Systems
{
    public delegate void KeyPressCallback(KeyInfo args);

    public struct KeyInfo
	{
		Key Key;
		byte ModCode;
		long Time;

		public KeyInfo(Key key, byte modCode, long time)
		{
			(Key, ModCode, Time) = (key, modCode, time);
		}
	}
}