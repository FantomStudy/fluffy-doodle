# AGENTS.md

## Purpose

This file defines the engineering rules and coding conventions for agents and contributors working in this project.

**Stack:**

- React
- Vite
- TypeScript

**Core styling rules:**

- Use **only CSS Modules**
- Use **only design tokens** defined as CSS variables in `index.css`
- Do not use inline hardcoded colors, spacing, radius, shadows, or typography values when a token exists

---

## General Principles

- Prefer simple, readable, and maintainable code
- Keep components and functions small and focused
- Avoid premature abstractions
- Prefer explicitness over cleverness
- Follow existing project structure and naming conventions
- Before introducing a new pattern, check whether the project already has an established one

---

## TypeScript

- Use strict TypeScript-friendly solutions
- Prefer explicit types when they improve readability
- Avoid `any`
- Prefer `unknown` over `any` when the type is not yet known
- Use narrow unions and reusable domain types where possible
- Keep component props typed close to the component
- Export shared types only when they are actually reused

---

## React Rules

- Use **arrow function definitions** for components and helpers

**Example:**

```ts
const ProfileCard = ({ name }: ProfileCardProps) => {
  if (!name) return null;

  return <div>{name}</div>;
};
```

- Prefer named exports unless the file clearly represents a route/page entry where default export is already the project convention
- Keep JSX clean and shallow where possible
- Move complex conditional rendering into variables or helper functions only if it improves readability
- Do not create unnecessary wrappers or fragments

---

## Early Returns

Prefer early returns instead of deep nesting.

**Good:**

```ts
const getLabel = (status: Status) => {
  if (status === "idle") return "Idle";
  if (status === "loading") return "Loading";
  if (status === "success") return "Success";

  return "Unknown";
};
```

**Avoid:**

```ts
const getLabel = (status: Status) => {
  if (status === "idle") {
    return "Idle";
  } else {
    if (status === "loading") {
      return "Loading";
    } else {
      if (status === "success") {
        return "Success";
      } else {
        return "Unknown";
      }
    }
  }
};
```

**Rules:**

- Return early for invalid state
- Return early for loading/error/empty states
- Avoid unnecessary `else` after `return`
- Reduce nesting depth whenever possible

---

## Styling

### Allowed

- CSS Modules
- CSS variables / design tokens from `index.css`

### Not Allowed

- Styled-components
- Emotion
- Tailwind
- Sass modules unless already explicitly configured and approved
- Inline styles for regular UI styling
- Hardcoded design values when a token exists

### Styling Rules

- Every component style must live in a corresponding `*.module.css` file
- Use class names that describe purpose, not appearance
- Use tokens from `index.css` for:
  - colors
  - spacing
  - typography
  - border radius
  - shadows
  - transitions
  - z-index if available

**Good:**

```css
.card {
  padding: var(--space-4);
  border-radius: var(--radius-md);
  background: var(--surface);
  color: var(--foreground);
}
```

**Avoid:**

```css
.card {
  padding: 16px;
  border-radius: 12px;
  background: #ffffff;
  color: #111111;
}
```

---

## CSS Modules Conventions

- File name format: `ComponentName.module.css`
- Import as:

```ts
import styles from "./ComponentName.module.css";
```

- Use:

```tsx
<div className={styles.root} />
```

- Prefer common structural names inside modules:
  - `root`
  - `container`
  - `content`
  - `header`
  - `title`
  - `description`
  - `actions`
  - `icon`
- Avoid global selectors unless absolutely necessary
- Avoid element selectors when a class selector is clearer
- Keep nesting minimal

---

## Design Tokens

- Treat `index.css` as the source of truth for design tokens
- Reuse existing tokens before introducing new ones
- If a new token is needed, prefer adding it to `index.css` rather than hardcoding a value in a module
- Do not introduce one-off visual values without strong reason

**When writing styles:**

1. Check whether a token already exists
2. Use the token
3. Only propose a new token if the value is truly reusable

---

## Component Design

- One component = one responsibility
- Prefer composition over large monolithic components
- Extract subcomponents only when:
  - logic is reusable
  - JSX becomes hard to read
  - a meaningful UI boundary exists
- Do not extract purely for the sake of reducing line count

### Component Structure

```
imports
types
constants
component
local helper functions if needed
export
```

**Example:**

```tsx
import styles from "./UserBadge.module.css";

type UserBadgeProps = {
  name: string;
  isOnline?: boolean;
};

const UserBadge = ({ name, isOnline = false }: UserBadgeProps) => {
  if (!name) return null;

  return (
    <div className={styles.root}>
      <span className={styles.name}>{name}</span>
      {isOnline && <span className={styles.status}>Online</span>}
    </div>
  );
};

export { UserBadge };
```

---

## Props

- Keep props minimal
- Do not pass data a component does not need
- Prefer explicit props over large ambiguous config objects unless there is a strong reason
- Avoid boolean prop explosions such as `isPrimary`, `isSecondary`, `isCompact`, `isLarge`, `isDanger`

Prefer a single variant prop:

```ts
type ButtonVariant = "primary" | "secondary" | "danger";
```

---

## State and Logic

- Keep local UI state close to the component that owns it
- Lift state only when necessary
- Avoid derived state when it can be computed during render
- Prefer clear event handler names: `handleClick`, `handleSubmit`, `handleChange`
- Avoid overly abstract hooks
- Extract custom hooks only when logic is truly reusable or significantly improves clarity

---

## Conditional Rendering

Prefer readable conditions and early returns.

**Good:**

```tsx
if (isLoading) return <Spinner />;
if (error) return <ErrorMessage message={error} />;
if (!items.length) return <EmptyState />;

return <List items={items} />;
```

**Avoid:**

- Deeply nested ternaries
- Large boolean expressions directly in JSX
- Mixing many UI states in one return block

---

## Functions

- Prefer arrow functions
- Keep functions short and single-purpose
- Use descriptive names
- Prefer pure functions when possible

**Good:**

```ts
const formatPrice = (value: number) => {
  if (value <= 0) return "Free";

  return `${value} RUB`;
};
```

**Avoid:**

- Large functions with multiple responsibilities
- Hidden side effects
- Unnecessary mutation

---

## Imports

- Keep imports organized
- Remove unused imports
- Prefer relative imports or project aliases according to existing project setup
- Do not create barrel files unless they are already part of the project architecture and provide real value

---

## File Naming

Prefer consistent naming based on file purpose.

**Examples:** `UserCard.tsx`, `UserCard.module.css`, `useUserFilters.ts`, `formatDate.ts`

**Conventions:**

- `PascalCase` for React components
- `camelCase` for hooks, utilities, and variables

---

## Accessibility

- Use semantic HTML first
- Buttons must be real `<button>` elements when they perform actions
- Links must be real `<a>` elements when they navigate
- Provide `alt` for meaningful images
- Preserve keyboard accessibility
- Do not remove focus styles unless they are replaced with an accessible alternative
- Use `aria-*` only when native semantics are insufficient

---

## Performance

- Do not optimize prematurely
- Memoization is allowed only when there is a clear benefit
- Avoid unnecessary re-renders caused by unstable object/array literals when it actually matters
- Prefer simpler code over speculative micro-optimizations

---

## Testing Mindset

When changing code:

- Preserve existing behavior unless the task requires changing it
- Cover edge cases
- Think through loading, error, empty, and success states
- Verify accessibility and responsiveness where relevant

---

## What Agents Should Avoid

- Do not introduce new styling systems
- Do not hardcode colors, spacing, or radii if tokens exist
- Do not use deep nesting when early returns can simplify the code
- Do not switch component style conventions inside the same codebase
- Do not introduce abstractions without clear payoff
- Do not add dependencies for trivial problems
- Do not rewrite unrelated code while implementing a focused task

---

## Preferred Output Style for Code Changes

When generating or editing code:

- Use arrow functions
- Prefer early returns
- Use CSS Modules for styling
- Use design tokens from `index.css`
- Keep code concise and readable
- Match existing architecture
- Avoid unnecessary comments
- Avoid large refactors unless explicitly requested

---

## Decision Rules for Agents

When multiple implementations are possible, prefer the one that:

- Follows existing project conventions
- Uses early returns
- Uses arrow functions
- Keeps logic simple
- Uses CSS Modules
- Uses design tokens from `index.css`
- Minimizes unnecessary abstraction

---

## Short Example

```tsx
import styles from "./StatusMessage.module.css";

type StatusMessageProps = {
  isLoading: boolean;
  error?: string | null;
  message?: string | null;
};

const StatusMessage = ({ isLoading, error, message }: StatusMessageProps) => {
  if (isLoading) return <p className={styles.root}>Loading...</p>;
  if (error) return <p className={styles.error}>{error}</p>;
  if (!message) return null;

  return <p className={styles.root}>{message}</p>;
};

export { StatusMessage };
```

```css
.root {
  color: var(--foreground);
  font-size: var(--font-size-md);
}

.error {
  color: var(--danger);
  font-size: var(--font-size-md);
}
```
