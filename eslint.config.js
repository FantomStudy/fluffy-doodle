import antfu from "@antfu/eslint-config";
import pluginQuery from "@tanstack/eslint-plugin-query";

const eslintConfig = antfu(
  {
    stylistic: false,
    markdown: false,
    react: true,
    ignores: ["package.json"],
  },
  {
    name: "fantomstudy/rewrite",
    rules: {
      "antfu/top-level-function": "off",
      "no-console": "off",
      "node/prefer-global/process": "off",
      "react/no-array-index-key": "off",
      "react-hooks/exhaustive-deps": "off",
      "react-refresh/only-export-components": "off",
      "eslint-comments/no-unlimited-disable": "off",
    },
  },
  ...pluginQuery.configs["flat/recommended"],
  {
    name: "fantomstudy/perfectionist",
    rules: {
      "perfectionist/sort-imports": [
        "error",
        {
          groups: [
            "type-import",
            ["type-parent", "type-sibling", "type-index", "type-internal"],
            "value-builtin",
            "value-external",
            "value-internal",
            ["value-parent", "value-sibling", "value-index"],
            "side-effect",
            "style",
            "side-effect-style",
            "ts-equals-import",
            "unknown",
          ],
          internalPattern: ["^~/.*", "^@/.*"],
          newlinesBetween: "ignore",
          order: "asc",
          type: "natural",
        },
      ],
    },
  },
);

export default eslintConfig;
