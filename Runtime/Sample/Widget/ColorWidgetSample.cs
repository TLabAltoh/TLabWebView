using UnityEngine;
using UnityEngine.UI;

namespace TLab.WebView.Widget.Sample
{
    public class ColorWidgetSample : MonoBehaviour, IWidgetHandler<ColorWidget.Init, ColorWidget>
    {
        [SerializeField] private GameObject m_content;
        [SerializeField] private Slider m_r;
        [SerializeField] private Slider m_g;
        [SerializeField] private Slider m_b;
        [SerializeField] private Image m_image;

        private ColorWidget m_picker;

        public void OnSliderChanged()
        {
            m_picker.rgb = new Color(m_r.value, m_g.value, m_b.value);
            m_image.color = m_picker.rgb;
        }

        public void Close()
        {
            m_picker = null;
            m_content.SetActive(false);
        }

        public void OnDialog(ColorWidget.Init init, ColorWidget widget)
        {
            m_picker = widget;

            var rgb = ColorWidget.UnMarshall(init.color);
            m_r.value = rgb.r;
            m_g.value = rgb.g;
            m_b.value = rgb.b;

            OnSliderChanged();

            m_content.SetActive(true);
        }
    }
}
