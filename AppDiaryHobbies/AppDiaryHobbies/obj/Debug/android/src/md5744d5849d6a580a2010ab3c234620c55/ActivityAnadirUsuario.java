package md5744d5849d6a580a2010ab3c234620c55;


public class ActivityAnadirUsuario
	extends android.app.Activity
	implements
		mono.android.IGCUserPeer
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"n_onCreate:(Landroid/os/Bundle;)V:GetOnCreate_Landroid_os_Bundle_Handler\n" +
			"";
		mono.android.Runtime.register ("AppDiaryHobbies.ActivityAnadirUsuario, AppDiaryHobbies", ActivityAnadirUsuario.class, __md_methods);
	}


	public ActivityAnadirUsuario ()
	{
		super ();
		if (getClass () == ActivityAnadirUsuario.class)
			mono.android.TypeManager.Activate ("AppDiaryHobbies.ActivityAnadirUsuario, AppDiaryHobbies", "", this, new java.lang.Object[] {  });
	}


	public void onCreate (android.os.Bundle p0)
	{
		n_onCreate (p0);
	}

	private native void n_onCreate (android.os.Bundle p0);

	private java.util.ArrayList refList;
	public void monodroidAddReference (java.lang.Object obj)
	{
		if (refList == null)
			refList = new java.util.ArrayList ();
		refList.add (obj);
	}

	public void monodroidClearReferences ()
	{
		if (refList != null)
			refList.clear ();
	}
}
