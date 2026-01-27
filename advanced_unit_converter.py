# advanced_unit_converter.py
import streamlit as st
import pandas as pd
from datetime import datetime
import io
import json

# ── Conversion Factors (Expanded Categories) ─────────────────────────────────
LENGTH = {
    "Meters": 1.0, "Kilometers": 0.001, "Centimeters": 100.0, "Millimeters": 1000.0,
    "Inches": 39.3701, "Feet": 3.28084, "Yards": 1.09361, "Miles": 0.000621371
}
WEIGHT = {
    "Kilograms": 1.0, "Grams": 1000.0, "Milligrams": 1000000.0,
    "Pounds": 2.20462, "Ounces": 35.274, "Tons (metric)": 0.001
}
TEMPERATURE = {
    "Celsius": lambda c: c,
    "Fahrenheit": lambda c: (c * 9/5) + 32,
    "Kelvin": lambda c: c + 273.15
}
AREA = {
    "Square Meters": 1.0, "Square Kilometers": 1e-6, "Square Centimeters": 10000.0,
    "Acres": 0.000247105, "Hectares": 0.0001, "Square Feet": 10.7639
}
VOLUME = {
    "Liters": 1.0, "Milliliters": 1000.0, "Cubic Meters": 0.001,
    "Gallons (US)": 0.264172, "Pints": 2.11338, "Cups": 4.22675
}
SPEED = {
    "Meters/Second": 1.0, "Kilometers/Hour": 3.6, "Miles/Hour": 2.23694,
    "Knots": 1.94384, "Feet/Second": 3.28084
}
TIME = {
    "Seconds": 1.0, "Minutes": 1/60, "Hours": 1/3600, "Days": 1/86400,
    "Weeks": 1/604800, "Years": 1/31536000
}
ENERGY = {
    "Joules": 1.0, "Kilojoules": 0.001, "Calories": 0.239006,
    "Kilocalories": 0.000239006, "Watt-Hours": 0.000277778
}
CURRENCY = {  # Static rates (USD base, as of 2026 sample; update manually)
    "USD": 1.0, "EUR": 0.92, "GBP": 0.78, "INR": 83.5, "NPR": 133.0,  # NPR for Nepal relevance
    "JPY": 145.0, "CAD": 1.35
}

CATEGORIES = {
    "Length": LENGTH, "Weight": WEIGHT, "Temperature": TEMPERATURE,
    "Area": AREA, "Volume": VOLUME, "Speed": SPEED,
    "Time": TIME, "Energy": ENERGY, "Currency": CURRENCY
}

# ── Session State Initialization ─────────────────────────────────────────────
if "history" not in st.session_state:
    st.session_state.history = []
if "favorites" not in st.session_state:
    st.session_state.favorites = []
if "theme" not in st.session_state:
    st.session_state.theme = "Light"  # Options: Light, Dark, Auto

# ── Theme Switcher (Enhanced with Auto Mode) ─────────────────────────────────
def set_theme():
    if st.session_state.theme == "Dark":
        st.markdown("""
            <style>
            section[data-testid="stSidebar"] { background: #1e1e1e; }
            .stApp { background: #121212; color: #e0e0e0; transition: background 0.3s; }
            </style>
        """, unsafe_allow_html=True)
    elif st.session_state.theme == "Auto":
        st.markdown("""
            <style>
            @media (prefers-color-scheme: dark) {
                .stApp { background: #121212; color: #e0e0e0; }
                section[data-testid="stSidebar"] { background: #1e1e1e; }
            }
            </style>
        """, unsafe_allow_html=True)
    else:
        st.markdown("<style>.stApp { transition: background 0.3s; }</style>", unsafe_allow_html=True)

set_theme()

# ── Main UI (Responsive and Enhanced) ────────────────────────────────────────
st.title("🔄 Enhanced Advanced Unit Converter")
st.markdown("**Categories:** Length, Weight, Temperature, Area, Volume, Speed, Time, Energy, Currency (Static Rates)  \n"
            "**Features:** Live Results, Searchable Units, Batch Mode, Charts, Exports, Themes  \n"
            f"**Local Time (Kathmandu):** {datetime.now().strftime('%Y-%m-%d %H:%M:%S')}")

tab_converter, tab_batch, tab_history, tab_favorites, tab_calc = st.tabs([
    "Converter", "Batch Convert", "History & Charts", "Favorites", "Quick Calculator"
])

with tab_converter:
    try:
        col_cat, col_from, col_arrow, col_to, col_value = st.columns([2, 2, 0.5, 2, 2])
        
        with col_cat:
            category = st.selectbox("Category 📏", list(CATEGORIES.keys()))
            factors = CATEGORIES[category]
            units = list(factors.keys())
        
        with col_from:
            from_search = st.text_input("Search From Unit", "")
            from_options = [u for u in units if from_search.lower() in u.lower()]
            from_unit = st.selectbox("From 🔍", from_options, label_visibility="collapsed")
        
        with col_arrow:
            st.markdown("<br>", unsafe_allow_html=True)
            if st.button("↔️ Swap"):
                # Reverse conversion
                temp = from_unit
                from_unit = to_unit
                to_unit = temp
                st.rerun()
        
        with col_to:
            to_search = st.text_input("Search To Unit", "")
            to_options = [u for u in units if to_search.lower() in u.lower()]
            to_unit = st.selectbox("To 🔍", to_options, label_visibility="collapsed")
        
        with col_value:
            value = st.number_input("Value 🔢", value=1.0, step=0.1, format="%.4f")
        
        # Live Result Calculation
        if category in ["Temperature"]:
            if from_unit == "Celsius":
                c = value
            elif from_unit == "Fahrenheit":
                c = (value - 32) * 5/9
            else:
                c = value - 273.15
            result = factors[to_unit](c)
            result_str = f"{value:.4f} {from_unit} = **{result:.4f} {to_unit}**"
        elif category == "Currency":
            result = value * (factors[to_unit] / factors[from_unit])
            result_str = f"{value:.4f} {from_unit} ≈ **{result:.4f} {to_unit}** (Static Rate)"
        else:
            result = value * (factors[to_unit] / factors[from_unit])
            result_str = f"{value:.4f} {from_unit} = **{result:.4f} {to_unit}**"
        
        st.markdown("### Result ✅")
        st.success(result_str)
        
        col_copy, col_hist, col_fav = st.columns(3)
        with col_copy:
            if st.button("📋 Copy Result"):
                st.write('<script>navigator.clipboard.writeText("' + result_str + '")</script>', unsafe_allow_html=True)
                st.success("Copied!")
        with col_hist:
            if st.button("💾 Save to History"):
                st.session_state.history.append({
                    "time": datetime.now().strftime("%Y-%m-%d %H:%M:%S"),
                    "category": category,
                    "conversion": f"{value:.4f} {from_unit} → {result:.4f} {to_unit}"
                })
                st.success("Saved!")
        with col_fav:
            if st.button("⭐ Add to Favorites"):
                conv = f"{category}: {value:.2f} {from_unit} → {to_unit}"
                if conv not in st.session_state.favorites:
                    st.session_state.favorites.append(conv)
                    st.success("Added!")
        
        # Suggested Conversions (Simple "AI-like" feature)
        st.expander("Suggested Common Conversions").markdown(
            "- 1 Meter → Feet  \n- 1 Kilogram → Pounds  \n- 0 Celsius → Fahrenheit"
        )
    except KeyError:
        st.error("Invalid unit selection. Please choose valid units.")
    except ValueError:
        st.error("Invalid input value. Please enter a number.")

with tab_batch:
    st.markdown("### Batch Conversion 📊")
    batch_data = [{"Value": 1.0, "From": units[0], "To": units[1]}] if 'batch_data' not in st.session_state else st.session_state.batch_data
    edited_df = st.data_editor(pd.DataFrame(batch_data), num_rows="dynamic", use_container_width=True)
    st.session_state.batch_data = edited_df.to_dict('records')
    
    if st.button("Convert Batch"):
        results = []
        for row in edited_df.to_dict('records'):
            v = row["Value"]
            fu = row["From"]
            tu = row["To"]
            if fu in factors and tu in factors:
                res = v * (factors[tu] / factors[fu])
                results.append(f"{v:.4f} {fu} → {res:.4f} {tu}")
            else:
                results.append("Invalid")
        st.write("Results:", "\n".join(results))

with tab_history:
    if st.session_state.history:
        df_hist = pd.DataFrame(st.session_state.history).sort_values("time", ascending=False)
        st.markdown("### Editable History ✏️")
        edited_hist = st.data_editor(df_hist, num_rows="dynamic", use_container_width=True)
        st.session_state.history = edited_hist.to_dict('records')
        
        # Chart: Category Distribution
        st.markdown("### History Analytics 📈")
        cat_counts = df_hist['category'].value_counts()
        st.pie_chart(cat_counts)
        
        # Export
        col_exp_csv, col_exp_json = st.columns(2)
        with col_exp_csv:
            csv = df_hist.to_csv(index=False).encode('utf-8')
            st.download_button("📥 Export CSV", csv, "history.csv", "text/csv")
        with col_exp_json:
            json_str = json.dumps(st.session_state.history)
            st.download_button("📥 Export JSON", json_str, "history.json", "application/json")
    else:
        st.info("No history yet. Perform conversions to build it.")

with tab_favorites:
    if st.session_state.favorites:
        st.markdown("### Favorites ⭐")
        fav_search = st.text_input("Search Favorites 🔍")
        filtered_favs = [f for f in st.session_state.favorites if fav_search.lower() in f.lower()]
        for i, item in enumerate(filtered_favs):
            col_fav_text, col_fav_del = st.columns([4, 1])
            with col_fav_text:
                st.write(f"• {item}")
            with col_fav_del:
                if st.button("❌", key=f"del_fav_{i}"):
                    st.session_state.favorites.remove(item)
                    st.rerun()
    else:
        st.info("No favorites yet. Add from Converter tab.")

with tab_calc:
    st.markdown("### Quick Calculator ➕")
    expr = st.text_input("Enter expression (e.g., 2 + 3 * 4)", "")
    if st.button("Calculate"):
        try:
            result = eval(expr)
            st.success(f"Result: {result}")
        except Exception as e:
            st.error(f"Error: {str(e)}")

# ── Sidebar Settings (Enhanced) ──────────────────────────────────────────────
with st.sidebar:
    st.header("Settings ⚙️")
    st.session_state.theme = st.radio("Theme 🌗", ["Light", "Dark", "Auto"])
    set_theme()
    
    st.markdown("---")
    if st.button("🗑️ Clear History"):
        st.session_state.history = []
        st.success("History cleared!")
    if st.button("🗑️ Clear Favorites"):
        st.session_state.favorites = []
        st.success("Favorites cleared!")
    
    st.markdown("---")
    st.caption("Install as PWA: Use browser menu to add to home screen.")
    st.caption("College Project • Streamlit v1.XX • Enhanced 2026")

st.caption("Developed with modern UI/UX best practices • Offline-ready • Responsive Design")
