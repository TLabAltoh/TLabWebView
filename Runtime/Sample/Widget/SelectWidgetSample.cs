using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

namespace TLab.WebView.Widget.Sample
{
    public class SelectWidgetSample : MonoBehaviour, IWidgetHandler<SelectWidget.Init, SelectWidget>
    {
        [SerializeField] private GameObject m_content;
        [SerializeField] private ScrollRect m_scroll;

        [Header("Item")]
        [SerializeField] private GameObject m_selectable;
        [SerializeField] private GameObject m_group;
        [SerializeField] private GameObject m_separator;

        private string THIS_NAME => "[" + this.GetType() + "] ";

        public void Close() => m_content.SetActive(false);

        private Button.ButtonClickedEvent Lambda2Event(UnityAction action)
        {
            var tmp = new Button.ButtonClickedEvent();
            tmp.AddListener(action);
            return tmp;
        }

        private UnityAction m_clear;

        private void ClearChild(Transform target)
        {
            for (int i = 0; i < target.childCount; i++)
                Destroy(target.GetChild(i).gameObject);
        }

        public void OnDialog(SelectWidget.Init init, SelectWidget widget)
        {
            ClearChild(m_scroll.content);

            var items = new GameObject[init.options.Length];

            m_clear = () =>
            {
                for (int i = 0; i < init.options.Length; i++)
                {
                    var option = init.options[i];

                    switch (option.type)
                    {
                        case SelectWidget.ModifiableChoice.Type.DEFAULT:
                            {
                                items[i].GetComponent<Image>().color = Color.white;
                            }
                            break;
                    }
                }
            };

            for (int i = 0; i < init.options.Length; i++)
            {
                var option = init.options[i];

                switch (option.type)
                {
                    case SelectWidget.ModifiableChoice.Type.DEFAULT:
                        {
                            var tmp = i;
                            items[i] = Instantiate(m_selectable, m_scroll.content);
                            items[i].GetComponentInChildren<TMPro.TextMeshProUGUI>().text = option.label;
                            items[i].GetComponent<Image>().color = option.selected ? Color.gray : Color.white;
                            items[i].GetComponent<Button>().onClick = Lambda2Event(() => {
                                if (widget.type == SelectWidget.Type.MENU || widget.type == SelectWidget.Type.SINGLE)
                                    m_clear.Invoke();

                                items[tmp].GetComponent<Image>().color = widget.Push(tmp) ? Color.gray : Color.white;
                            });
                        }
                        break;
                    case SelectWidget.ModifiableChoice.Type.GROUP:
                        {
                            items[i] = Instantiate(m_group, m_scroll.content);
                            items[i].GetComponentInChildren<TMPro.TextMeshProUGUI>().text = option.label;
                            continue;
                        }
                    case SelectWidget.ModifiableChoice.Type.SEPARATOR:
                        {
                            items[i] = Instantiate(m_separator, m_scroll.content);
                            continue;
                        }
                }
            }

            m_content.SetActive(true);
        }
    }
}
