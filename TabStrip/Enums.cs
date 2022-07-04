using System.Drawing;
namespace FarsiLibrary.Win {
    /// <summary>
    /// Hit test result of <see cref="FATabStrip"/>
    /// </summary>
    public enum HitTestResult {
        CloseButton,
        MenuGlyph,
        TabItem,
        None
    }

    /// <summary>
    /// Theme Type
    /// </summary>
    public enum ThemeTypes {
        WindowsXP,
        Office2000,
        Office2003
    }

    /// <summary>
    /// Indicates a change into TabStrip collection
    /// </summary>
    public enum FATabStripItemChangeTypes {
        Added,
        Removed,
        Changed,
        SelectionChanged
    }
    
  
    //public class _CONST {
    
    //    //public static Color TAB_ACTIVE_TITLE_BACKGROUND_1 = SystemColors.ControlLightLight;
    //    //public static Color TAB_ACTIVE_TITLE_BACKGROUND_2 = SystemColors.Window;
    //    //public static Color TAB_ACTIVE_TOOLBAR_BACKGROUND = SystemColors.Window;

    //    public static Color TAB_ACTIVE_TITLE_BACKGROUND_1 = Color.White;
    //    public static Color TAB_ACTIVE_TITLE_BACKGROUND_2 = Color.LightSkyBlue;
    //    public static Color TAB_ACTIVE_TOOLBAR_BACKGROUND = SystemColors.Control;//Color.Red;
    //    public static Color TAB_ACTIVE_BORDER_COLOR = Color.FromArgb(127, 157, 185);// Color.Gray;//, Color.FromArgb(164, 185, 127), Color.FromArgb(165, 172, 178), Color.FromArgb(132, 130, 132)
    //    //private static Pen _Pen_TAB_ACTIVE_BORDER_COLOR;
    //    //public static Pen Pen_TAB_ACTIVE_BORDER_COLOR {
    //    //    get {
    //    //        if(_Pen_TAB_ACTIVE_BORDER_COLOR == null) {
    //    //            _Pen_TAB_ACTIVE_BORDER_COLOR = new Pen(TAB_ACTIVE_BORDER_COLOR);
    //    //        }
    //    //        return _Pen_TAB_ACTIVE_BORDER_COLOR;
    //    //    }
    //    //}
    //}
}
