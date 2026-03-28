import type { Task } from "../api/courses";
import * as React from "react";
import { useState } from "react";
import { Pressable, ScrollView, StyleSheet, Text, View } from "react-native";
import { colors, radii, semantic } from "../theme/tokens";

type BlockShape = "oval" | "rect" | "para";
type BlockColor = "green" | "blue" | "purple" | "orange" | "red";

interface FlowBlock {
  id: string;
  shape: BlockShape;
  label: string;
  color: BlockColor;
}

const THEORY_SLIDES = [
  {
    icon: "🗺️",
    title: "Что такое алгоритм?",
    text: "Алгоритм — это точная инструкция, описанная шаг за шагом. Как рецепт!",
  },
  {
    icon: "🔷",
    title: "Формы блоков",
    text: "В блок-схеме каждый тип действия имеет свою форму.",
  },
  {
    icon: "➡️",
    title: "Стрелки — это порядок",
    text: "Стрелки соединяют блоки и показывают порядок выполнения.",
  },
  {
    icon: "🎯",
    title: "Твоё задание!",
    text: "Составь блок-схему: алгоритм умножения числа на 2. Расставь блоки в правильном порядке!",
  },
];

const ALL_BLOCKS: FlowBlock[] = [
  { id: "start", shape: "oval", label: "Начало", color: "green" },
  { id: "input", shape: "para", label: "Ввести число N", color: "blue" },
  { id: "calc", shape: "rect", label: "N × 2 = результат", color: "purple" },
  { id: "output", shape: "para", label: "Вывести результат", color: "orange" },
  { id: "end", shape: "oval", label: "Конец", color: "red" },
];

const PALETTE_ORDER = ["calc", "start", "output", "end", "input"];
const CORRECT_ORDER = ["start", "input", "calc", "output", "end"];

const blockColors: Record<BlockColor, { bg: string; border: string }> = {
  green: { bg: colors.green50, border: colors.green600 },
  blue: { bg: colors.purple50, border: colors.purple600 },
  purple: { bg: "#f5f3ff", border: "#7c3aed" },
  orange: { bg: colors.amber50, border: colors.amber500 },
  red: { bg: "#fef2f2", border: colors.red400 },
};

const shapeStyle = (shape: BlockShape) => {
  if (shape === "oval") return { borderRadius: 999 };
  if (shape === "para") return { borderRadius: radii.md, transform: [{ skewX: "-5deg" }] as any };
  return { borderRadius: radii.md };
};

const TheoryPhase = ({ onFinish }: { onFinish: () => void }) => {
  const [idx, setIdx] = useState(0);
  const slide = THEORY_SLIDES[idx];
  const isLast = idx === THEORY_SLIDES.length - 1;

  return (
    <View style={styles.theoryShell}>
      <View style={styles.progressBar}>
        <View
          style={[
            styles.progressFill,
            { width: `${((idx + 1) / THEORY_SLIDES.length) * 100}%` as any },
          ]}
        />
      </View>

      <ScrollView contentContainerStyle={styles.theoryBody}>
        <Text style={styles.theoryIcon}>{slide.icon}</Text>
        <Text style={styles.theoryStep}>
          Шаг {idx + 1} из {THEORY_SLIDES.length}
        </Text>
        <Text style={styles.theoryTitle}>{slide.title}</Text>
        <Text style={styles.theoryText}>{slide.text}</Text>
      </ScrollView>

      <View style={styles.navRow}>
        {idx > 0 ? (
          <Pressable style={styles.navBtn} onPress={() => setIdx(idx - 1)}>
            <Text style={styles.navBtnText}>← Назад</Text>
          </Pressable>
        ) : (
          <View />
        )}
        <View style={styles.dots}>
          {THEORY_SLIDES.map((_, i) => (
            <View key={i} style={[styles.dot, i === idx && styles.dotActive]} />
          ))}
        </View>
        {isLast ? (
          <Pressable style={[styles.navBtn, styles.navBtnStart]} onPress={onFinish}>
            <Text style={styles.navBtnStartText}>Начать 🚀</Text>
          </Pressable>
        ) : (
          <Pressable style={[styles.navBtn, styles.navBtnNext]} onPress={() => setIdx(idx + 1)}>
            <Text style={styles.navBtnNextText}>Далее →</Text>
          </Pressable>
        )}
      </View>
    </View>
  );
};

const FlowchartTask = ({ task, onComplete }: { task?: Task; onComplete?: () => void }) => {
  const [workspace, setWorkspace] = useState<FlowBlock[]>([]);
  const [result, setResult] = useState<"idle" | "correct" | "wrong">("idle");

  const paletteBlocks = PALETTE_ORDER.map((id) => ALL_BLOCKS.find((b) => b.id === id)!).filter(
    (b) => !workspace.some((w) => w.id === b.id),
  );

  const addToWorkspace = (block: FlowBlock) => {
    setWorkspace((prev) => [...prev, block]);
    setResult("idle");
  };

  const removeFromWorkspace = (id: string) => {
    setWorkspace((prev) => prev.filter((b) => b.id !== id));
    setResult("idle");
  };

  const moveUp = (idx: number) => {
    if (idx === 0) return;
    setWorkspace((prev) => {
      const next = [...prev];
      [next[idx - 1], next[idx]] = [next[idx], next[idx - 1]];
      return next;
    });
    setResult("idle");
  };

  const moveDown = (idx: number) => {
    if (idx === workspace.length - 1) return;
    setWorkspace((prev) => {
      const next = [...prev];
      [next[idx], next[idx + 1]] = [next[idx + 1], next[idx]];
      return next;
    });
    setResult("idle");
  };

  const check = () => {
    if (workspace.length !== CORRECT_ORDER.length) {
      setResult("wrong");
      return;
    }
    const ok = CORRECT_ORDER.every((id, i) => workspace[i].id === id);
    setResult(ok ? "correct" : "wrong");
    if (ok) onComplete?.();
  };

  const reset = () => {
    setWorkspace([]);
    setResult("idle");
  };

  return (
    <ScrollView style={styles.taskShell} contentContainerStyle={styles.taskContent}>
      <Text style={styles.taskTitle}>
        📊 {task?.title ?? "Блок-схема: алгоритм умножения на 2"}
      </Text>
      <Text style={styles.taskDesc}>
        {task?.description ??
          "Нажми на блок в палитре, чтобы добавить. Используй ↑↓ для перемещения. × для удаления."}
      </Text>

      <Text style={styles.sectionLabel}>Палитра блоков</Text>
      <View style={styles.paletteList}>
        {paletteBlocks.map((block) => {
          const c = blockColors[block.color];
          return (
            <Pressable
              key={block.id}
              style={[
                styles.flowBlock,
                { backgroundColor: c.bg, borderColor: c.border },
                shapeStyle(block.shape),
              ]}
              onPress={() => addToWorkspace(block)}
            >
              <Text style={[styles.flowBlockText, { color: c.border }]}>{block.label}</Text>
            </Pressable>
          );
        })}
        {paletteBlocks.length === 0 && <Text style={styles.emptyMsg}>Все блоки добавлены!</Text>}
      </View>

      <View style={styles.legendRow}>
        <Text style={styles.legendItem}>⬭ Начало/Конец</Text>
        <Text style={styles.legendItem}>▭ Действие</Text>
        <Text style={styles.legendItem}>▱ Ввод/Вывод</Text>
      </View>

      <Text style={styles.sectionLabel}>Твоя блок-схема</Text>
      {workspace.length === 0 ? (
        <View style={styles.flowEmpty}>
          <Text style={styles.flowEmptyIcon}>📦</Text>
          <Text style={styles.flowEmptyText}>Добавь блоки из палитры</Text>
        </View>
      ) : (
        workspace.map((block, i) => {
          const c = blockColors[block.color];
          return (
            <View key={block.id}>
              <View style={styles.wsBlockRow}>
                <View
                  style={[
                    styles.flowBlock,
                    styles.wsBlock,
                    { backgroundColor: c.bg, borderColor: c.border },
                    shapeStyle(block.shape),
                  ]}
                >
                  <Text style={[styles.flowBlockText, { color: c.border }]}>{block.label}</Text>
                </View>
                <View style={styles.wsActions}>
                  <Pressable
                    onPress={() => moveUp(i)}
                    disabled={i === 0}
                    style={[styles.wsActionBtn, i === 0 && styles.wsActionBtnDisabled]}
                  >
                    <Text>↑</Text>
                  </Pressable>
                  <Pressable
                    onPress={() => moveDown(i)}
                    disabled={i === workspace.length - 1}
                    style={[
                      styles.wsActionBtn,
                      i === workspace.length - 1 && styles.wsActionBtnDisabled,
                    ]}
                  >
                    <Text>↓</Text>
                  </Pressable>
                  <Pressable onPress={() => removeFromWorkspace(block.id)} style={styles.removeBtn}>
                    <Text style={styles.removeBtnText}>×</Text>
                  </Pressable>
                </View>
              </View>
              {i < workspace.length - 1 && (
                <View style={styles.connArrow}>
                  <Text style={styles.arrowText}>↓</Text>
                </View>
              )}
            </View>
          );
        })
      )}

      {result === "correct" && (
        <Text style={styles.successBanner}>✅ Отлично! Блок-схема правильная! +10 XP</Text>
      )}
      {result === "wrong" && (
        <Text style={styles.errorBanner}>❌ Порядок неверный. Попробуй снова!</Text>
      )}

      <View style={styles.footerBtns}>
        <Pressable style={styles.btnReset} onPress={reset}>
          <Text style={styles.btnResetText}>🔄 Сбросить</Text>
        </Pressable>
        <Pressable
          style={[styles.btnCheck, workspace.length === 0 && styles.btnDisabled]}
          onPress={check}
          disabled={workspace.length === 0}
        >
          <Text style={styles.btnCheckText}>✅ Проверить</Text>
        </Pressable>
      </View>
    </ScrollView>
  );
};

interface FlowchartChallengeProps {
  task?: Task;
  onComplete?: () => void;
}

export const FlowchartChallenge = ({ task, onComplete }: FlowchartChallengeProps) => {
  const [phase, setPhase] = useState<"theory" | "task">("theory");

  return phase === "theory" ? (
    <TheoryPhase onFinish={() => setPhase("task")} />
  ) : (
    <FlowchartTask task={task} onComplete={onComplete} />
  );
};

const styles = StyleSheet.create({
  theoryShell: { flex: 1, backgroundColor: semantic.background },
  progressBar: {
    height: 6,
    backgroundColor: semantic.borderSubtle,
    borderRadius: radii.full,
    marginHorizontal: 16,
    marginTop: 12,
  },
  progressFill: { height: "100%", backgroundColor: colors.purple600, borderRadius: radii.full },
  theoryBody: { padding: 24, alignItems: "center", gap: 12 },
  theoryIcon: { fontSize: 48 },
  theoryStep: { fontSize: 13, color: semantic.foregroundMuted },
  theoryTitle: { fontSize: 24, fontWeight: "700", color: semantic.foreground, textAlign: "center" },
  theoryText: {
    fontSize: 16,
    color: semantic.foregroundStrong,
    textAlign: "center",
    lineHeight: 24,
  },
  navRow: {
    flexDirection: "row",
    justifyContent: "space-between",
    alignItems: "center",
    padding: 16,
    gap: 8,
  },
  navBtn: { paddingHorizontal: 16, paddingVertical: 10 },
  navBtnText: { fontSize: 14, color: semantic.foregroundMuted },
  navBtnNext: {
    backgroundColor: colors.purple600,
    borderRadius: radii.full,
    paddingHorizontal: 20,
  },
  navBtnNextText: { fontSize: 14, fontWeight: "600", color: semantic.foregroundWhite },
  navBtnStart: {
    backgroundColor: colors.purple600,
    borderRadius: radii.full,
    paddingHorizontal: 20,
  },
  navBtnStartText: { fontSize: 14, fontWeight: "600", color: semantic.foregroundWhite },
  dots: { flexDirection: "row", gap: 6 },
  dot: { width: 8, height: 8, borderRadius: 4, backgroundColor: semantic.borderSubtle },
  dotActive: { backgroundColor: colors.purple600 },
  taskShell: { flex: 1, backgroundColor: semantic.background },
  taskContent: { padding: 16, gap: 16, paddingBottom: 100 },
  taskTitle: { fontSize: 20, fontWeight: "700", color: semantic.foreground },
  taskDesc: { fontSize: 14, color: semantic.foregroundMuted, lineHeight: 20 },
  sectionLabel: { fontSize: 14, fontWeight: "700", color: semantic.foreground },
  paletteList: { gap: 8 },
  flowBlock: { borderWidth: 2, paddingVertical: 12, paddingHorizontal: 20, alignItems: "center" },
  flowBlockText: { fontSize: 14, fontWeight: "600" },
  wsBlock: { flex: 1 },
  emptyMsg: { fontSize: 13, color: semantic.foregroundMuted, textAlign: "center" },
  legendRow: { flexDirection: "row", gap: 12, flexWrap: "wrap" },
  legendItem: { fontSize: 12, color: semantic.foregroundMuted },
  flowEmpty: {
    alignItems: "center",
    padding: 32,
    backgroundColor: semantic.surface,
    borderRadius: radii.xl,
    borderWidth: 2,
    borderColor: semantic.borderSubtle,
    borderStyle: "dashed",
  },
  flowEmptyIcon: { fontSize: 32 },
  flowEmptyText: { fontSize: 14, color: semantic.foregroundMuted },
  wsBlockRow: { flexDirection: "row", alignItems: "center", gap: 8 },
  wsActions: { flexDirection: "row", gap: 4 },
  wsActionBtn: {
    width: 32,
    height: 32,
    borderRadius: 16,
    backgroundColor: semantic.surface,
    borderWidth: 1,
    borderColor: semantic.border,
    alignItems: "center",
    justifyContent: "center",
  },
  wsActionBtnDisabled: { opacity: 0.3 },
  removeBtn: {
    width: 32,
    height: 32,
    borderRadius: 16,
    backgroundColor: "#fef2f2",
    alignItems: "center",
    justifyContent: "center",
  },
  removeBtnText: { fontSize: 18, color: colors.red400, fontWeight: "700" },
  connArrow: { alignItems: "center", paddingVertical: 4 },
  arrowText: { fontSize: 18, color: semantic.foregroundMuted },
  successBanner: {
    fontSize: 15,
    fontWeight: "700",
    color: colors.green600,
    textAlign: "center",
    backgroundColor: colors.green50,
    padding: 12,
    borderRadius: radii.xl,
    overflow: "hidden",
  },
  errorBanner: {
    fontSize: 15,
    fontWeight: "700",
    color: colors.red400,
    textAlign: "center",
    backgroundColor: "#fef2f2",
    padding: 12,
    borderRadius: radii.xl,
    overflow: "hidden",
  },
  footerBtns: { flexDirection: "row", gap: 10 },
  btnReset: {
    flex: 1,
    borderWidth: 1,
    borderColor: semantic.border,
    borderRadius: radii.full,
    paddingVertical: 12,
    alignItems: "center",
  },
  btnResetText: { fontSize: 14, fontWeight: "600", color: semantic.foreground },
  btnCheck: {
    flex: 1,
    backgroundColor: colors.purple600,
    borderRadius: radii.full,
    paddingVertical: 12,
    alignItems: "center",
  },
  btnCheckText: { fontSize: 14, fontWeight: "700", color: semantic.foregroundWhite },
  btnDisabled: { opacity: 0.5 },
});
