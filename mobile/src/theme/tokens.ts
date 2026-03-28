export const colors = {
  white: "#ffffff",

  slate50: "#f5f5f7",
  slate100: "#ebebf0",
  slate200: "#e8e8ef",

  gray50: "#f9fafb",
  gray100: "#f3f4f6",
  gray200: "#e5e7eb",
  gray400: "#9ca3af",
  gray500: "#6b7280",
  gray600: "#4b5563",
  gray700: "#374151",
  gray900: "#111827",

  purple50: "#f5f3ff",
  purple100: "#ede9fe",
  purple600: "#7c3aed",
  purple700: "#6d28d9",

  green50: "#f0fdf4",
  green100: "#dcfce7",
  green400: "#4ade80",
  green600: "#16a34a",
  green700: "#15803d",

  amber50: "#fffbeb",
  amber200: "#fde68a",
  amber500: "#f59e0b",
  amber900: "#92400e",

  orange500: "#ff7d24",

  red400: "#f87171",

  editorBg: "#1e2030",
  editorBgDark: "#161822",
  editorBorder: "#2a2d3e",
  editorText: "#c8d3f5",
  editorTextMuted: "#8892c8",
  editorTextSubtle: "#565f89",
  editorAccent: "#7986cb",
} as const;

export const semantic = {
  primary: colors.purple600,
  primaryHover: colors.purple700,
  primaryBg: colors.purple50,
  primaryBorder: colors.purple100,

  success: colors.green600,
  successHover: colors.green700,
  successBg: colors.green50,
  successBorder: colors.green100,
  successAccent: colors.green400,

  warningBg: colors.amber50,
  warningBorder: colors.amber200,
  warningIcon: colors.amber500,
  warningText: colors.amber900,

  accent: colors.orange500,
  danger: colors.red400,

  foreground: colors.gray900,
  foregroundStrong: colors.gray700,
  foregroundMuted: colors.gray500,
  foregroundSubtle: colors.gray400,
  foregroundWhite: colors.white,

  background: colors.slate50,
  surface: colors.white,
  surfaceMuted: colors.gray50,

  border: colors.gray200,
  borderSubtle: colors.gray100,
  borderLight: colors.slate100,
} as const;

export const radii = {
  sm: 4,
  md: 8,
  lg: 10,
  xl: 12,
  "2xl": 14,
  "3xl": 22,
  "4xl": 40,
  full: 999,
} as const;

export const fonts = {
  regular: "System",
  medium: "System",
  semiBold: "System",
  bold: "System",
  extraBold: "System",
} as const;
