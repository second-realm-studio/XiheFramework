namespace XiheFramework.Core.UI.UIEntity {
    public class UIPageEntity :UILayoutEntity{
        /// <summary>
        /// Is this page home page
        /// </summary>
        public bool homePage = false;

        /// <summary>
        /// destroy instead of hide
        /// </summary>
        public bool destroyOnClose = true;
    }
}