# unit_converter.py
import streamlit as st
import pandas as pd
from datetime import datetime

# ── Conversion factors ───────────────────────────────────────────────────────
LENGTH = {
    "Meters": 1.0,
    "Kilometers": 0.001,
    "Centimeters": 100.0,
    "Millimeters": 1000.0,
    "Inches": 39.3701,
    "Feet": 3.28084,
    "Yards": 1.09361,
    "Miles": 0.000621371
}

WEIGHT = {
    "Kilograms": 1.0,
    "Grams": 1000.0,
    "Milligrams": 1000000.0,
    "Pounds": 2.20462,
    "Ounces": 35.274,
    "Tons (metric)": 0.001
}

TEMPERATURE = {
    "Celsius": lambda c: c,
    "Fahrenheit": lambda c: (c * 9/5) + 32,
    "Kelvin": lambda c: c + 273.15
}

# ── Session state initialization ─────────────────────────────────────────────
if "history" not in st.session_state:
    st.session_state.history = []
if "favorites" not in st.session_state:
    st.session_state.favorites = []
if "theme" not in st.session_state:
    st.session_state.theme = "Light"

# ── Theme switcher ───────────────────────────────────────────────────────────
def set_theme():
    if st.session_state.theme == "Dark":
        st.markdown("""
            <style>
            section[data-testid="stSidebar"] { background: #1e1e1e; }
            .stApp { background: #121212; color: #e0e0e0; }
            </style>
        """, unsafe_allow_html=True)
    else:
        st.markdown("<style></style>", unsafe_allow_html=True)  # reset

set_theme()

# ── Main UI ──────────────────────────────────────────────────────────────────
st.title("Advanced Unit Converter")
st.markdown("Length • Weight • Temperature • History • Favorites • Themes")

tab1, tab2, tab3 = st.tabs(["Converter", "History", "Favorites"])

with tab1:
    col1, col2, col3 = st.columns([2, 1, 2])

    with col1:
        category = st.selectbox("Category", ["Length", "Weight", "Temperature"])

        if category == "Length":
            units = list(LENGTH.keys())
            factors = LENGTH
        elif category == "Weight":
            units = list(WEIGHT.keys())
            factors = WEIGHT
        else:
            units = list(TEMPERATURE.keys())
            factors = TEMPERATURE

        from_unit = st.selectbox("From", units, key="from")
        to_unit = st.selectbox("To", units, key="to")

    with col2:
        st.markdown("<br><br>", unsafe_allow_html=True)
        st.markdown("→")

    with col3:
        value = st.number_input("Value", value=1.0, step=0.1, format="%.4f")

    # ── Calculate result ─────────────────────────────────────────────────────
    if category in ["Length", "Weight"]:
        result = value * (factors[to_unit] / factors[from_unit])
        result_str = f"{value:.4f} {from_unit} = **{result:.4f} {to_unit}**"
    else:  # Temperature (special case)
        # First convert to Celsius
        if from_unit == "Celsius":
            c = value
        elif from_unit == "Fahrenheit":
            c = (value - 32) * 5/9
        else:  # Kelvin
            c = value - 273.15

        # Then to target
        result = factors[to_unit](c)
        result_str = f"{value:.2f} {from_unit} = **{result:.2f} {to_unit}**"

    st.markdown("### Result")
    st.success(result_str)

    # Add to history
    if st.button("Save to History"):
        st.session_state.history.append({
            "time": datetime.now().strftime("%Y-%m-%d %H:%M:%S"),
            "category": category,
            "conversion": f"{value:.4f} {from_unit} → {result:.4f} {to_unit}"
        })
        st.success("Saved!")

    # Add to favorites
    if st.button("Add to Favorites"):
        conv = f"{category}: {value:.2f} {from_unit} → {to_unit}"
        if conv not in st.session_state.favorites:
            st.session_state.favorites.append(conv)
            st.success("Added to favorites!")

with tab2:
    if st.session_state.history:
        df = pd.DataFrame(st.session_state.history)
        st.dataframe(df.sort_values("time", ascending=False), use_container_width=True)
    else:
        st.info("No conversions saved yet.")

with tab3:
    if st.session_state.favorites:
        for item in st.session_state.favorites:
            st.write(f"• {item}")
    else:
        st.info("No favorites yet.")

# ── Settings sidebar ─────────────────────────────────────────────────────────
with st.sidebar:
    st.header("Settings")
    st.session_state.theme = st.radio("Theme", ["Light", "Dark"])
    set_theme()  # apply immediately

    if st.button("Clear History"):
        st.session_state.history = []
        st.success("History cleared!")
    if st.button("Clear Favorites"):
        st.session_state.favorites = []
        st.success("Favorites cleared!")

st.caption("College project • Streamlit • No external dependencies • 2026")
