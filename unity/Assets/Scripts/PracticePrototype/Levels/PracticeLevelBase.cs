using UnityEngine;
using UnityEngine.UI;

namespace PracticePrototype
{
    internal abstract class PracticeLevelBase : IPracticeLevel
    {
        protected PracticeRuntimeContext Context;
        protected RectTransform Root;
        protected bool IsBusy;

        public abstract PracticeLevelId LevelId { get; }
        public abstract string Title { get; }
        public abstract string Description { get; }

        public virtual void Build(PracticeRuntimeContext context)
        {
            Context = context;
            Root = PracticeUiFactory.CreateUIObject(GetType().Name, Context.UI.GameArea).GetComponent<RectTransform>();
            PracticeUiFactory.AddLayoutElement(Root.gameObject, flexibleHeight: 1f, flexibleWidth: 1f);
            Root.gameObject.SetActive(false);
        }

        public virtual void Show()
        {
            Root.gameObject.SetActive(true);
            Context.UI.BindNavigation(RunLevel, ResetLevel, Context.ShowSelector);
            Context.Bootstrap.HideResult();
        }

        public virtual void Hide()
        {
            if (Root != null)
            {
                Root.gameObject.SetActive(false);
            }
        }

        public abstract void ResetLevel();
        public abstract void RunLevel();
        public virtual void Tick() { }

        public virtual void Dispose()
        {
            if (Root != null)
            {
                Object.Destroy(Root.gameObject);
            }
        }

        protected Text CreateSectionTitle(Transform parent, string text)
        {
            var label = PracticeUiFactory.CreateText("SectionTitle", parent, Context.DefaultFont, 22, TextAnchor.UpperLeft, PracticeUiFactory.TextPrimary, text);
            PracticeUiFactory.AddLayoutElement(label.gameObject, preferredHeight: 32f);
            return label;
        }

        protected Text CreateBodyText(Transform parent, string text, int size = 18)
        {
            return PracticeUiFactory.CreateText("BodyText", parent, Context.DefaultFont, size, TextAnchor.UpperLeft, PracticeUiFactory.TextSecondary, text);
        }
    }
}
