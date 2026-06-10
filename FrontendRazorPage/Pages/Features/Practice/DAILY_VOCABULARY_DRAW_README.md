# 🎮 Daily Vocabulary Draw (Gacha/Lootbox Style)

## Overview
A modern, interactive Japanese vocabulary learning feature that presents vocabulary items as mystery cards in a Gacha/Lootbox style interface. Users can click cards to reveal vocabulary with a smooth 3D flip animation.

---

## 📋 Architecture

### Backend (C# - PageModel)
**File:** `Index.cshtml.cs`

```csharp
public class IndexModel : PageModel
{
	private readonly VocabularyClientService _vocabularyService;
	public List<DailyVocabularyItemDto> DailyVocabulary { get; set; }
}
```

#### Key Components:

1. **DailyVocabularyItemDto**
   - Data transfer object for vocabulary items
   - Properties: `Id`, `Kanji`, `Hiragana`, `Meaning`

2. **OnGetAsync() Method**
   - Fetches vocabulary from the API via `VocabularyClientService`
   - Randomly selects 5 items from available vocabulary
   - Uses `OrderBy(x => _random.Next()).Take(5)` for randomization
   - Gracefully handles errors with empty list fallback

#### Data Flow:
```
API (Backend) 
  → VocabularyClientService 
  → OnGetAsync() 
  → DailyVocabulary List 
  → Razor View (@Model.DailyVocabulary)
```

---

## 🎨 Frontend (Razor View + CSS + Vanilla JS)

### File Structure:
- **HTML**: Card layout with Bootstrap 5 grid system
- **CSS**: 3D flip animations, responsive design, theming
- **JavaScript**: Pure Vanilla JS for card interaction

### Layout Breakdown:

#### 1. **Header Section**
```html
<div class="page-header">
	<div class="page-title">🎮 日本語 GACHA</div>
	<div class="page-subtitle">Today's Vocabulary Draw</div>
</div>
```
- Displays the Gacha theme title
- Shows drawing information

#### 2. **Cards Container**
```html
<div class="cards-container">
	<div class="cards-grid">
		<!-- Flip cards rendered here -->
	</div>
</div>
```
- Responsive grid layout (auto-fit columns, 180px minimum)
- Wraps elegantly on mobile devices

#### 3. **Individual Flip Card**
```html
<div class="flip-card" onclick="flipCard(this)">
	<div class="flip-card-inner">
		<!-- Front (Face Down) -->
		<div class="flip-card-front">
			<!-- Mystery icon, text, and hint -->
		</div>

		<!-- Back (Face Up) -->
		<div class="flip-card-back">
			<!-- Kanji, Hiragana, Meaning -->
		</div>
	</div>
</div>
```

---

## 🔄 3D Flip Animation

### CSS Implementation:

```css
.flip-card-inner {
	transition: transform 0.6s cubic-bezier(0.68, -0.55, 0.265, 1.55);
	transform-style: preserve-3d;
}

.flip-card-inner.flipped {
	transform: rotateY(180deg);
}

.flip-card-front,
.flip-card-back {
	backface-visibility: hidden;
	position: absolute;
}

.flip-card-back {
	transform: rotateY(180deg);
}
```

#### Animation Details:
- **Perspective**: `perspective: 1000px` creates 3D depth
- **Timing Function**: Custom cubic-bezier for smooth, bouncy feel
- **Duration**: 0.6 seconds
- **Technique**: CSS 3D transforms with `rotateY(180deg)`

### How It Works:
1. Card starts with `transform: rotateY(0deg)` - front face visible
2. On click, `.flipped` class is added
3. CSS transition smoothly rotates to `rotateY(180deg)` - back face visible
4. `backface-visibility: hidden` hides the back side while rotating

---

## 💻 JavaScript Logic

### Core Functions:

#### 1. **flipCard(cardElement)**
```javascript
function flipCard(cardElement) {
	const flipInner = cardElement.querySelector('.flip-card-inner');
	flipInner.classList.toggle('flipped');
}
```
- Toggles the `.flipped` class on the card
- Triggers CSS animation automatically
- Allows repeated flips (toggle behavior)

#### 2. **resetAllCards()**
```javascript
function resetAllCards() {
	const allFlipInners = document.querySelectorAll('.flip-card-inner');
	allFlipInners.forEach(flipInner => {
		flipInner.classList.remove('flipped');
	});
}
```
- Resets all cards to face-down state
- Triggered by "Reset Cards" button or pressing 'R' key

#### 3. **Keyboard Interaction**
```javascript
document.addEventListener('keydown', (event) => {
	if (event.key.toLowerCase() === 'r') {
		resetAllCards();
	}
});
```
- Press 'R' to reset all cards
- Enhances user experience for power users

---

## 🎯 Card Design

### Front (Face Down - Mystery Side):
- **Icon**: Question mark with pulsing animation
- **Text**: "Mystery" label
- **Hint**: "Click to Reveal" instruction
- **Background**: Dark gradient with diagonal stripe pattern
- **Animation**: Icon pulses to draw attention

### Back (Face Up - Content Side):
- **Kanji**: Large text (2.5rem) - main vocabulary character
- **Hiragana**: Reading of the Kanji
- **Meaning**: Vietnamese translation
- **Background**: Cyan gradient (sci-fi theme)
- **Color**: Dark text on bright background for readability

---

## 📱 Responsive Design

### Breakpoints:

#### Desktop (> 768px)
- 5 columns in a row (or fewer if space allows)
- Card height: 250px
- Title: 2.5rem font-size

#### Tablet (≤ 768px)
- Adaptive grid: `minmax(150px, 1fr)`
- Card height: 200px
- Title: 1.8rem font-size

#### Mobile (≤ 480px)
- 2-3 cards per row
- Card height: 180px
- Title: 1.5rem font-size
- Reduced padding and gaps

---

## 🎮 User Interactions

### Click Actions:
1. **Click Face-Down Card** → Flips to show vocabulary
2. **Click Flipped Card Again** → Flips back to face-down
3. **"Draw Again" Button** → Reloads page for new random draw
4. **"Reset Cards" Button** → Hides all content without reload
5. **Press 'R' Key** → Quick reset of all cards

### Hover Effects:
- Card scales up slightly (1.02)
- Border color changes to yellow (secondary color)
- Shadow intensity increases
- Mystery icon grows larger

---

## 🔧 Setup & Configuration

### Prerequisites:
- VocabularyClientService registered in dependency injection
- API endpoint returning vocabulary data
- Bootstrap 5 CSS library loaded

### Integration Steps:

1. **Register Service** (in Startup or Program.cs):
```csharp
services.AddScoped<VocabularyClientService>();
```

2. **Add Namespace** (in _ViewImports.cshtml if needed):
```csharp
@using FrontendRazorPage.Pages.Features.Practice
```

3. **Customize API Base** (in VocabularyClientService):
```csharp
private readonly string _apiBase = "https://localhost:7104/api/vocabulary";
```

---

## 🎨 Customization

### Change Colors:
```css
:root {
	--primary-color: #00d4ff;      /* Main accent color */
	--secondary-color: #ffff00;    /* Highlight color */
	--dark-bg: #0f1419;            /* Dark background */
	--card-bg: #1a1f2e;            /* Card background */
}
```

### Adjust Animation Speed:
```css
transition: transform 0.6s cubic-bezier(...);  /* Change 0.6s to desired duration */
```

### Modify Grid Layout:
```css
grid-template-columns: repeat(auto-fit, minmax(180px, 1fr));
/* Change minmax value to adjust card width */
```

### Change Card Dimensions:
```css
.flip-card {
	height: 250px;  /* Adjust height */
}
```

---

## 📊 Performance Considerations

### Optimizations:
1. **CSS 3D Transforms**: Hardware-accelerated animations
2. **Lazy Rendering**: Only visible cards are processed
3. **Event Delegation**: Single click handler on parent
4. **No External Dependencies**: Pure vanilla JavaScript
5. **Responsive Grid**: CSS Grid handles layout (no JS recalculation)

### File Sizes:
- Backend (.cs): ~2.5 KB
- Frontend (HTML/CSS/JS): ~25 KB (inline, no external deps)

---

## 🧪 Testing Checklist

- [ ] Cards render correctly from API data
- [ ] Clicking a card flips it smoothly
- [ ] Flipping again returns to face-down
- [ ] "Reset Cards" button resets all cards
- [ ] "Draw Again" loads new random vocabulary
- [ ] 'R' key resets cards
- [ ] Responsive on mobile (test 375px, 768px, 1024px)
- [ ] Empty state displays when no vocabulary available
- [ ] Hover effects work on desktop
- [ ] Touch events work on mobile (onclick is touch-friendly)

---

## 🚀 Future Enhancements

1. **Sound Effects**: Add flip sound using Web Audio API
2. **Difficulty Levels**: Different vocabulary sets (N1, N2, N3, etc.)
3. **Daily Login Rewards**: XP/Points for completing daily draw
4. **Sharing**: Share results on social media
5. **Statistics**: Track vocabulary learned percentage
6. **Favorites**: Mark vocabulary as favorite
7. **Retry Limit**: Limited draws per day (like real gacha)
8. **Animations**: Particle effects on successful reveals
9. **Modes**: Mix Kanji, Hiragana, Meaning for different learning styles
10. **Database Integration**: Direct DB access for faster loading

---

## 📚 Code Quality Notes

### Why This Approach Works:
1. **Clean Separation**: Backend fetches, frontend displays
2. **Reusable Service**: `VocabularyClientService` used by other features
3. **Responsive**: CSS Grid + media queries, no JavaScript layouts
4. **Accessible**: Semantic HTML, meaningful alt text for icons
5. **Performance**: No DOM manipulation loops, event bubbling
6. **Maintainable**: Well-commented, clear function names
7. **University-Ready**: Clean, educational code structure

---

## 🎓 Learning Outcomes

Students will understand:
- ✅ ASP.NET Core Razor Pages architecture
- ✅ Frontend-backend data binding
- ✅ CSS 3D transforms and animations
- ✅ Vanilla JavaScript DOM manipulation
- ✅ Responsive web design
- ✅ Git-friendly modular code structure
- ✅ API integration patterns
- ✅ Error handling and graceful degradation

---

**Created for University Project Presentation** 🎓
