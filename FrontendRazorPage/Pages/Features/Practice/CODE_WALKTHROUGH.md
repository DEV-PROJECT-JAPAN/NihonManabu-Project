# 🎮 Daily Vocabulary Draw - Code Walkthrough

## Complete Line-by-Line Explanation

---

## PART 1: Backend (Index.cshtml.cs)

### Imports & Namespaces
```csharp
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using FrontendRazorPage.Models;
using FrontendRazorPage.Core.Services;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
```

**Explanation**:
- `Microsoft.AspNetCore.Mvc` - Base MVC framework
- `RazorPages` - PageModel class for Razor Pages
- `Models` - VocabularyModel class
- `Services` - VocabularyClientService for API calls
- `LINQ` - For `.OrderBy()`, `.Take()`, `.Select()`
- `Threading.Tasks` - For `async/await` operations

---

### Data Transfer Object (DTO)
```csharp
public class DailyVocabularyItemDto
{
	public int Id { get; set; }
	public string Kanji { get; set; }
	public string Hiragana { get; set; }
	public string Meaning { get; set; }
}
```

**Why DTOs?**
- **Separation of Concerns**: Frontend only gets what it needs
- **Security**: Hide sensitive data (API URLs, extra fields)
- **Flexibility**: Can map multiple DB models to single DTO
- **Maintenance**: Changes to VocabularyModel don't break UI

**Properties**:
- `Id` - Unique identifier (optional, but good to have)
- `Kanji` - Japanese character(s)
- `Hiragana` - Phonetic reading
- `Meaning` - Vietnamese/English translation

---

### PageModel Class
```csharp
public class IndexModel : PageModel
{
	private readonly VocabularyClientService _vocabularyService;
	private readonly Random _random = new();

	public List<DailyVocabularyItemDto> DailyVocabulary { get; set; } = new();
}
```

**Components**:

1. **Inherits from PageModel**
   - Required for Razor Pages
   - Provides `OnGet()`, `OnPost()`, `Page()` methods

2. **_vocabularyService** (Private, Readonly)
   - Injected via constructor dependency injection
   - Makes HTTP calls to backend API
   - Readonly = immutable reference (best practice)

3. **_random** (Private, Readonly)
   - `Random` instance for generating random numbers
   - Pre-instantiated so it's available to methods

4. **DailyVocabulary** (Public Property)
   - Auto-implemented property with initializer
   - `= new()` ensures it's never null (no null reference exceptions)
   - Bound to view via `@Model.DailyVocabulary`

---

### Constructor
```csharp
public IndexModel(VocabularyClientService vocabularyService)
{
	_vocabularyService = vocabularyService;
}
```

**How It Works**:
1. ASP.NET Core DI container sees constructor parameter
2. Looks up registered service: `services.AddScoped<VocabularyClientService>()`
3. Creates instance and passes it here
4. Stored in `_vocabularyService` for use in `OnGetAsync()`

**Benefits**:
- Testability (can inject mock service)
- Loose coupling (doesn't create service itself)
- Centralized configuration (in Startup/Program.cs)

---

### OnGetAsync() Method
```csharp
public async Task OnGetAsync()
{
	try
	{
		// Fetch lessons from level 1
		var lessons = await _vocabularyService.GetLessonsAsync(1);

		if (lessons.Any())
		{
			// Get first lesson
			var firstLesson = lessons.First();

			// Fetch vocabulary from that lesson
			var allVocab = await _vocabularyService.GetCardsAsync(firstLesson.Id);

			if (allVocab.Any())
			{
				// Random select 5 items, map to DTO
				DailyVocabulary = allVocab
					.OrderBy(x => _random.Next())
					.Take(5)
					.Select(v => new DailyVocabularyItemDto
					{
						Id = v.Id,
						Kanji = v.Kanji,
						Hiragana = v.Hiragana,
						Meaning = v.Meaning
					})
					.ToList();
			}
		}
	}
	catch (Exception ex)
	{
		Console.WriteLine($"Error fetching daily vocabulary: {ex.Message}");
		DailyVocabulary = new List<DailyVocabularyItemDto>();
	}
}
```

**Line-by-Line Breakdown**:

#### 1. Async Method Signature
```csharp
public async Task OnGetAsync()
```
- `async` keyword - method can await asynchronous operations
- `Task` return type - caller can `await` this method
- `OnGetAsync()` - Razor Pages convention for GET requests
- Must be `async Task` (not `async void`)

#### 2. Try-Catch Block
```csharp
try { ... }
catch (Exception ex) { ... }
```
- Wraps entire method in error handler
- Catches ANY exception (network errors, parsing, etc.)
- Prevents application crash if API fails

#### 3. Fetch Lessons
```csharp
var lessons = await _vocabularyService.GetLessonsAsync(1);
```
- `await` pauses execution until HTTP request completes
- `GetLessonsAsync(1)` - Get lessons from level 1
- `var` - Type inferred as `List<LessonModel>`
- Result stored in `lessons` variable

#### 4. Check if Data Exists
```csharp
if (lessons.Any())
{
	var firstLesson = lessons.First();
```
- `Any()` - LINQ method returns `true` if list has items
- `First()` - Gets first item in list
- Prevents null reference if `lessons` is empty

#### 5. Fetch Vocabulary from Lesson
```csharp
var allVocab = await _vocabularyService.GetCardsAsync(firstLesson.Id);
```
- `GetCardsAsync()` - Fetch vocabulary for specific lesson
- Passes lesson ID from `firstLesson`
- Awaits async HTTP call

#### 6. Check Vocabulary Exists
```csharp
if (allVocab.Any())
{
```
- Ensures we have items to randomize
- Prevents errors on empty result

#### 7. Randomization & Selection
```csharp
DailyVocabulary = allVocab
	.OrderBy(x => _random.Next())
	.Take(5)
	.Select(v => new DailyVocabularyItemDto { ... })
	.ToList();
```

**LINQ Chain Explanation**:

a) **OrderBy(x => _random.Next())**
   ```csharp
   .OrderBy(x => _random.Next())
   ```
   - Sorts by random numbers
   - `_random.Next()` generates random int (0 to Int32.MaxValue)
   - Each item gets assigned random sort key
   - Result: random order

b) **Take(5)**
   ```csharp
   .Take(5)
   ```
   - After randomizing, take only first 5
   - If fewer than 5 items exist, returns all

c) **Select() - Projection**
   ```csharp
   .Select(v => new DailyVocabularyItemDto
   {
	   Id = v.Id,
	   Kanji = v.Kanji,
	   Hiragana = v.Hiragana,
	   Meaning = v.Meaning
   })
   ```
   - Transforms `VocabularyModel` → `DailyVocabularyItemDto`
   - Maps properties from source to DTO
   - Could filter/modify here if needed

d) **ToList()**
   ```csharp
   .ToList();
   ```
   - Executes entire query
   - Converts `IEnumerable` to `List<T>`
   - Now ready to use in view

#### 8. Error Handling
```csharp
catch (Exception ex)
{
	Console.WriteLine($"Error fetching daily vocabulary: {ex.Message}");
	DailyVocabulary = new List<DailyVocabularyItemDto>();
}
```
- Catches all exceptions during data fetch
- Logs error message to console
- Sets `DailyVocabulary` to empty list (prevents null errors in view)
- View will display "No Vocabulary Available" message

---

## PART 2: Frontend (Index.cshtml)

### HTML - Razor Page Declaration
```html
@page
@model FrontendRazorPage.Pages.Features.Practice.IndexModel
@{
	ViewData["Title"] = "Daily Vocabulary Draw - Gacha Style";
}
```

**Components**:
- `@page` - Tells ASP.NET this is a Razor Page
- `@model` - Binds to `IndexModel` class
- `@{ }` - C# code block
- `ViewData["Title"]` - Sets page title in browser

---

### Page Header
```html
<div class="page-header">
	<div class="page-title">
		<i class="fas fa-dice"></i> 日本語 GACHA
	</div>
	<div class="page-subtitle">Today's Vocabulary Draw</div>
	<div class="draw-info">✨ Draw 5 Random Words ✨</div>
</div>
```

**Elements**:
- `.page-header` - Container for title area
- `.page-title` - Main heading (with dice icon)
- `.page-subtitle` - Subheading
- `.draw-info` - Additional info text

---

### Card Container Markup
```html
<div class="cards-container">
	@if (Model.DailyVocabulary != null && Model.DailyVocabulary.Count > 0)
	{
		<div class="cards-grid">
			@foreach (var vocab in Model.DailyVocabulary)
			{
				<!-- Individual card markup -->
			}
		</div>
	}
	else
	{
		<!-- Empty state -->
	}
</div>
```

**Logic**:
- `@if (Model.DailyVocabulary != null && Model.DailyVocabulary.Count > 0)`
  - Server-side check: only render grid if data exists
  - Prevents null reference errors
  - Ensures count > 0 before looping

- `@foreach (var vocab in Model.DailyVocabulary)`
  - Iterates through each vocabulary item
  - `vocab` - current item in loop
  - Renders card HTML 5 times

- `else` block
  - Displays when no vocabulary available
  - Shows friendly "No Vocabulary Available" message

---

### Individual Flip Card
```html
<div class="flip-card" onclick="flipCard(this)">
	<div class="flip-card-inner">
		<!-- Front (Face Down) -->
		<div class="flip-card-front">
			<div class="flip-card-front-content">
				<div class="mystery-icon">
					<i class="fas fa-question"></i>
				</div>
				<div class="mystery-text">Mystery</div>
				<div class="flip-hint">Click to Reveal</div>
			</div>
		</div>

		<!-- Back (Face Up - Content) -->
		<div class="flip-card-back">
			<div class="card-kanji">@vocab.Kanji</div>
			<div class="card-hiragana">@vocab.Hiragana</div>
			<div class="card-meaning">@vocab.Meaning</div>
		</div>
	</div>
</div>
```

**Structure**:
```
flip-card (click handler)
├── flip-card-inner (rotated by JS)
│   ├── flip-card-front (initial visible)
│   │   └── Question mark icon + text
│   └── flip-card-back (pre-rotated 180°)
│       ├── Kanji (@vocab.Kanji)
│       ├── Hiragana (@vocab.Hiragana)
│       └── Meaning (@vocab.Meaning)
```

**Interaction Flow**:
1. User clicks `.flip-card` div
2. `onclick="flipCard(this)"` calls JS function
3. Passes the clicked element as `this`
4. JS toggles `.flipped` class on `.flip-card-inner`
5. CSS animation rotates from 0° to 180°
6. Back content becomes visible

---

## PART 3: CSS Animations

### 3D Perspective Setup
```css
.flip-card {
	perspective: 1000px;
}
```

**What it does**:
- Creates 3D space for child elements
- Value in pixels = "distance" to viewer
- Lower = more dramatic perspective
- 1000px = typical for subtle 3D effect

---

### Transform Container
```css
.flip-card-inner {
	transition: transform 0.6s cubic-bezier(0.68, -0.55, 0.265, 1.55);
	transform-style: preserve-3d;
}

.flip-card-inner.flipped {
	transform: rotateY(180deg);
}
```

**Breakdown**:

1. **transition**
   ```css
   transition: transform 0.6s cubic-bezier(0.68, -0.55, 0.265, 1.55);
   ```
   - Animates `transform` property
   - Duration: 0.6 seconds
   - Easing function: cubic-bezier (custom timing curve)
   - Cubic-bezier values:
	 - `0.68` - starts slow
	 - `-0.55` - overshoots (bouncy effect)
	 - `0.265` - easing in
	 - `1.55` - landing

2. **transform-style: preserve-3d**
   - Keeps 3D context for child elements
   - Allows back face to rotate "behind" front

3. **.flipped state**
   - `transform: rotateY(180deg)` - rotate around Y axis
   - 180 degrees = complete flip
   - Triggers transition automatically

---

### Front & Back Cards
```css
.flip-card-front {
	backface-visibility: hidden;
	position: absolute;
}

.flip-card-back {
	backface-visibility: hidden;
	position: absolute;
	transform: rotateY(180deg);
}
```

**Key Properties**:

1. **backface-visibility: hidden**
   - Hides element when rotated away from viewer
   - Front becomes invisible at 180°
   - Back becomes visible at 180°
   - Creates illusion of actual flip

2. **position: absolute**
   - Stacks both cards in same space
   - Only one visible at a time

3. **Back card pre-rotation**
   ```css
   .flip-card-back {
	   transform: rotateY(180deg);
   }
   ```
   - Back starts already rotated 180°
   - Hidden by `backface-visibility` initially
   - Becomes visible when parent rotates 180°

---

## PART 4: JavaScript

### Flip Card Function
```javascript
function flipCard(cardElement) {
	const flipInner = cardElement.querySelector('.flip-card-inner');
	flipInner.classList.toggle('flipped');
}
```

**Step-by-Step**:

1. **Parameter**
   ```javascript
   function flipCard(cardElement) {
   ```
   - Called with `this` from `onclick="flipCard(this)"`
   - `cardElement` = the `.flip-card` div that was clicked

2. **Query Selector**
   ```javascript
   const flipInner = cardElement.querySelector('.flip-card-inner');
   ```
   - Finds first `.flip-card-inner` child
   - Stores reference in `flipInner` variable

3. **Toggle Class**
   ```javascript
   flipInner.classList.toggle('flipped');
   ```
   - `.toggle('flipped')` adds or removes class
   - If present → removes it
   - If absent → adds it
   - CSS transition automatically animates

**Why This Approach?**
- ✅ No direct style manipulation (cleaner code)
- ✅ CSS handles all animations (GPU accelerated)
- ✅ Easy to debug (just check if class exists)
- ✅ Reusable across components

---

### Reset All Cards Function
```javascript
function resetAllCards() {
	const allFlipInners = document.querySelectorAll('.flip-card-inner');
	allFlipInners.forEach(flipInner => {
		flipInner.classList.remove('flipped');
	});
}
```

**Breakdown**:

1. **Select All Cards**
   ```javascript
   const allFlipInners = document.querySelectorAll('.flip-card-inner');
   ```
   - `querySelectorAll()` returns NodeList of ALL matches
   - Different from `querySelector()` which returns first only

2. **Iterate Each Card**
   ```javascript
   allFlipInners.forEach(flipInner => {
	   flipInner.classList.remove('flipped');
   });
   ```
   - `forEach()` loops through NodeList
   - `classList.remove('flipped')` removes class from each
   - All cards flip back to face-down simultaneously

---

### Keyboard Interaction
```javascript
document.addEventListener('keydown', (event) => {
	if (event.key.toLowerCase() === 'r') {
		resetAllCards();
	}
});
```

**How It Works**:

1. **Event Listener**
   ```javascript
   document.addEventListener('keydown', ...);
   ```
   - Listens for any key press on page
   - `keydown` = fired when key pressed (once)

2. **Event Check**
   ```javascript
   if (event.key.toLowerCase() === 'r') {
   ```
   - `event.key` = which key was pressed
   - `.toLowerCase()` = handles 'R' and 'r'
   - Checks if it's 'r'

3. **Function Call**
   ```javascript
   resetAllCards();
   ```
   - Calls reset function to flip all cards back

**Accessibility Benefit**:
- Keyboard power users can quickly reset without clicking button
- Improves accessibility for users with motor disabilities

---

## CSS Responsive Breakpoints

### Desktop (> 768px)
```css
.cards-grid {
	grid-template-columns: repeat(auto-fit, minmax(180px, 1fr));
	gap: 2rem;
}
```
- `minmax(180px, 1fr)` - cards 180px or flex fill
- Can fit 5+ cards per row
- Large gaps (2rem)

### Tablet (≤ 768px)
```css
@@media (max-width: 768px) {
	.cards-grid {
		grid-template-columns: repeat(auto-fit, minmax(150px, 1fr));
		gap: 1.5rem;
	}
}
```
- Slightly smaller cards (150px)
- Smaller gaps (1.5rem)
- 3-4 cards per row

### Mobile (≤ 480px)
```css
@@media (max-width: 480px) {
	.cards-grid {
		grid-template-columns: repeat(auto-fit, minmax(130px, 1fr));
		gap: 1rem;
	}
}
```
- Compact cards (130px)
- Minimal gaps (1rem)
- 2-3 cards per row

**Why `auto-fit`?**
- Automatically creates columns as many as fit
- No need for hardcoded "2 per row" media query
- Adapts to ANY screen size seamlessly

---

## Complete Data Flow Diagram

```
┌─────────────────────┐
│ User Visits /Practice
└──────────┬──────────┘
		   │
		   ▼
┌─────────────────────────────────┐
│ ASP.NET Core                    │
│ ├─ OnGetAsync() called          │
│ ├─ Fetch from API               │
│ └─ Store in DailyVocabulary    │
└──────────┬──────────────────────┘
		   │
		   ▼
┌──────────────────────────────────────┐
│ Razor View Renders                   │
│ ├─ Check if DailyVocabulary.Any()   │
│ ├─ @foreach render 5 cards           │
│ └─ Bind @vocab.Kanji, etc.          │
└──────────┬───────────────────────────┘
		   │
		   ▼
┌──────────────────────────────────────┐
│ HTML Sent to Browser                 │
│ ├─ 5 flip-card divs with data        │
│ ├─ onclick handlers attached         │
│ └─ CSS animations loaded             │
└──────────┬───────────────────────────┘
		   │
		   ▼
┌──────────────────────────────────────┐
│ User Interaction                     │
│ ├─ Click card                        │
│ └─ onclick="flipCard(this)" triggered│
└──────────┬───────────────────────────┘
		   │
		   ▼
┌──────────────────────────────────────┐
│ JavaScript Processing               │
│ ├─ querySelector finds .flip-card-inner
│ ├─ classList.toggle('flipped')      │
│ └─ Browser detects class change     │
└──────────┬───────────────────────────┘
		   │
		   ▼
┌──────────────────────────────────────┐
│ CSS Animation                        │
│ ├─ transition: 0.6s detected        │
│ ├─ rotateY(0) → rotateY(180deg)     │
│ └─ Front fades, back appears        │
└──────────┬───────────────────────────┘
		   │
		   ▼
┌──────────────────────────────────────┐
│ Card Displays Content                │
│ ├─ Kanji visible                     │
│ ├─ Hiragana visible                  │
│ └─ Meaning visible                   │
└──────────────────────────────────────┘
```

---

## Summary: Why This Architecture Works

### ✅ Clean Separation of Concerns
- Backend: Data fetching & preparation
- Frontend: Presentation & interaction
- No business logic in view
- No HTML generation in backend

### ✅ Performance
- CSS animations (GPU accelerated)
- No page reloads for card flips
- Minimal JavaScript (event handlers only)
- Responsive grid (CSS, no JS calculations)

### ✅ Maintainability
- Clear folder structure
- Documented code
- Easy to extend (add sounds, effects, etc.)
- Easy to test (can mock service)

### ✅ User Experience
- Smooth 3D animations
- Intuitive interaction model
- Mobile responsive
- Keyboard accessible

---

**This architecture is production-ready and scalable!** 🚀
