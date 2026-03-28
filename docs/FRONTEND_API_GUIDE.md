# Frontend API Integration Guide

This guide describes how to integrate with the Fluffy Doodle API.

## Base URL
The API base URL is `http://localhost:3000` (development).

## Authentication
The API uses HTTP-only cookies for authentication.
- **`access_token`**: Used for authorizing requests.
- **`refresh_token`**: Used to obtain a new access token when the old one expires.

**Important:** All frontend requests (using `fetch` or `axios`) MUST include `credentials: 'include'` (or `withCredentials: true` in axios) to send/receive these cookies.

---

## User Profile & Assets

### 1. Get My Profile (`GET /me`)
Returns the current authenticated user's data.
**Response Fields:**
- `id`, `login`, `fullName`, `phoneNumber`, `avatar`, `stars`, `exp`, `streak`, `level`, `expToNextLevel`, `roleName`.
- `activeFrame`: (Object) Currently equipped frame `{ id, name, price, image }`.
- `ownedFrames`: (Array of IDs) List of frame IDs owned by the user.
- `achievements`: (Array of Objects) List of unlocked achievements `{ id, name, description, icon }`.

### 2. Get User Profile (`GET /user/profile`)
Similar to `/me`, but if the user is a `parent`, it returns their linked `child`'s data.

### 3. Leaderboard (`GET /user/leaderboard/stars?limit=10`)
Returns users sorted by stars.
**Fields:** `id`, `fullName`, `avatar`, `stars`, `level`, `activeFrame`.

---

## Courses & Learning

### 1. Submit Task (`POST /courses/lessons/:lessonId/tasks/:taskId/submit`)
Submits a task solution.
**Response Fields:**
- `isSolved`: (boolean) Was the answer correct?
- `awardedStars`, `awardedExp`: Rewards gained (if solved first time).
- `awardedAchievements`: (Array of Objects) Any new achievements unlocked by this action (e.g., "Первый урок" when completing the last task of a lesson).

---

## Shop & Frames

### 1. Get All Frames (`GET /user/frames`)
Returns all frames available in the shop.
**Fields:** `id`, `name`, `price`, `image`, `owned` (boolean).

### 2. Buy Frame (`POST /user/frames/:frameId/buy`)
Purchases a frame using stars.
- Returns `200 OK` on success.
- Returns `400 Bad Request` if not enough stars or already owned.

### 3. Set Active Frame (`POST /user/frames/:frameId/active`)
Equips a frame. 
- Use `frameId = 0` to unequip (remove frame).
- Returns `200 OK` if the user owns the frame.

---

## Forum

### 1. Categories (`GET /forum/categories`)
List of forum sections.

### 2. Create Category (`POST /forum/categories`)
Create a new section (accessible to all authenticated users).
**Body:** `{ "name": "...", "description": "...", "order": 1 }`

### 3. Topics (`GET /forum/topics?categoryId=X`)
List of topics. Filter by `categoryId` if needed.
- Each topic includes `author` with their `activeFrame`.

### 4. Create Topic (`POST /forum/topics`)
**Body:** `{ "title": "...", "content": "...", "categoryId": 1 }`

### 5. Topic Details & Comments (`GET /forum/topics/:id`)
Returns the topic and all its comments.

### 6. Create Comment (`POST /forum/topics/:id/comments`)
**Body:** `{ "content": "..." }`

### 7. Mark as Solution (`PUT /forum/comments/:id/solution?topicId=X`)
Only the topic author can mark a comment as the solution.

---

## UI Implementation Tips

### Visualizing Stars & Levels
- Use the `level` field to show user progress.
- Use `exp` and `expToNextLevel` to show a progress bar to the next level.

### Avatar Frames
When displaying a user's avatar, if `activeFrame` is present, overlay the `activeFrame.image` over the avatar image.
- **Tip:** Use `position: absolute` for the frame image to place it exactly around the avatar.

### Achievements Popup
When `awardedAchievements` is not empty in the task submission response, show a congratulatory popup or animation to the user.

### Error Handling
All errors follow this format:
```json
{
  "success": false,
  "error": "Error message description"
}
```
