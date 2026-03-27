import {
  BookOpen,
  CheckCheck,
  ChevronLeft,
  ChevronRight,
  ClipboardList,
  Home,
  Map,
  RotateCcw,
  Trophy,
  User,
} from "lucide-react";
import { useEffect, useState } from "react";
import s from "./FlowchartChallenge.module.css";

// ─── Types ───────────────────────────────────────────────────────────────────

type BlockShape = "oval" | "rect" | "para";
type BlockColor = "green" | "blue" | "purple" | "orange" | "red";

export interface FlowBlock {
  id: string;
  shape: BlockShape;
  label: string;
  color: BlockColor;
}

// ─── Data ────────────────────────────────────────────────────────────────────

const THEORY_SLIDES = [
  {
    icon: "🗺️",
    title: "Что такое алгоритм?",
    text: "Алгоритм — это точная инструкция, описанная шаг за шагом. Как рецепт в кулинарной книге! Компьютер выполняет шаги строго по порядку — никаких пропусков.",
    visual: "recipe" as const,
  },
  {
    icon: "🔷",
    title: "Формы блоков",
    text: "В блок-схеме каждый тип действия имеет свою форму. По форме сразу видно, что делает блок — начало, действие или ввод данных.",
    visual: "legend" as const,
  },
  {
    icon: "➡️",
    title: "Стрелки — это порядок",
    text: "Стрелки соединяют блоки и показывают, что делать дальше. Компьютер идёт от блока к блоку строго по стрелкам — шаг за шагом!",
    visual: "arrows" as const,
  },
  {
    icon: "🎯",
    title: "Твоё задание!",
    text: "Составь блок-схему: алгоритм, который берёт число N и умножает его на 2. Перетащи блоки из палитры в рабочую область — расставь их в правильном порядке!",
  },
];

const ALL_BLOCKS: FlowBlock[] = [
  { id: "start",  shape: "oval", label: "Начало",            color: "green"  },
  { id: "input",  shape: "para", label: "Ввести число N",    color: "blue"   },
  { id: "calc",   shape: "rect", label: "N × 2 = результат", color: "purple" },
  { id: "output", shape: "para", label: "Вывести результат", color: "orange" },
  { id: "end",    shape: "oval", label: "Конец",             color: "red"    },
];

const PALETTE_BLOCKS = ["calc", "start", "output", "end", "input"].map(
  id => ALL_BLOCKS.find(b => b.id === id)!,
);

const CORRECT_ORDER = ["start", "input", "calc", "output", "end"];

// ─── Mini-visual components ──────────────────────────────────────────────────

function MiniBlock({ shape, color, children }: { shape: BlockShape; color: BlockColor; children: string }) {
  return (
    <div className={`${s.miniBlock} ${s[`miniShape_${shape}`]} ${s[`miniColor_${color}`]}`}>
      <span>{children}</span>
    </div>
  );
}

function MiniArrow() {
  return <div className={s.miniArrow} />;
}

function TheoryVisual({ visual }: { visual: "recipe" | "legend" | "arrows" }) {
  if (visual === "recipe") {
    return (
      <div className={s.miniFlow}>
        <MiniBlock shape="oval" color="green">Начало</MiniBlock>
        <MiniArrow />
        <MiniBlock shape="rect" color="blue">Смешать тесто</MiniBlock>
        <MiniArrow />
        <MiniBlock shape="rect" color="purple">Испечь</MiniBlock>
        <MiniArrow />
        <MiniBlock shape="oval" color="red">Конец</MiniBlock>
      </div>
    );
  }

  if (visual === "legend") {
    return (
      <div className={s.legendGrid}>
        <div className={s.legendItem}>
          <MiniBlock shape="oval" color="green">Начало / Конец</MiniBlock>
          <span className={s.legendItemLabel}>Начало или конец программы</span>
        </div>
        <div className={s.legendItem}>
          <MiniBlock shape="rect" color="purple">Действие</MiniBlock>
          <span className={s.legendItemLabel}>Вычисление или операция</span>
        </div>
        <div className={s.legendItem}>
          <MiniBlock shape="para" color="orange">Ввод / Вывод</MiniBlock>
          <span className={s.legendItemLabel}>Получить или показать данные</span>
        </div>
      </div>
    );
  }

  // arrows
  return (
    <div className={s.miniFlow}>
      <MiniBlock shape="oval" color="green">Начало</MiniBlock>
      <div className={s.miniArrowLabeled}>
        <div className={s.miniArrowLine} />
        <span className={s.miniArrowTag}>шаг 1</span>
        <div className={s.miniArrow} />
      </div>
      <MiniBlock shape="rect" color="blue">Взять число</MiniBlock>
      <div className={s.miniArrowLabeled}>
        <div className={s.miniArrowLine} />
        <span className={s.miniArrowTag}>шаг 2</span>
        <div className={s.miniArrow} />
      </div>
      <MiniBlock shape="para" color="orange">Вывести результат</MiniBlock>
      <div className={s.miniArrowLabeled}>
        <div className={s.miniArrowLine} />
        <span className={s.miniArrowTag}>шаг 3</span>
        <div className={s.miniArrow} />
      </div>
      <MiniBlock shape="oval" color="red">Конец</MiniBlock>
    </div>
  );
}

// ─── DropZone ────────────────────────────────────────────────────────────────

function DropZone({
  idx,
  active,
  show,
  onOver,
  onLeave,
  onDrop,
}: {
  idx: number;
  active: boolean;
  show: boolean;
  onOver: (e: React.DragEvent<HTMLDivElement>, i: number) => void;
  onLeave: () => void;
  onDrop: (e: React.DragEvent<HTMLDivElement>, i: number) => void;
}) {
  return (
    <div
      className={`${s.dropZone} ${show ? s.dropZoneShow : ""} ${active ? s.dropZoneActive : ""}`}
      onDragOver={e => onOver(e, idx)}
      onDragLeave={onLeave}
      onDrop={e => onDrop(e, idx)}
    />
  );
}

// ─── Sidebar ─────────────────────────────────────────────────────────────────

const NAV = [
  { icon: <Home size={16} />, label: "Главная" },
  { icon: <Map size={16} />, label: "Путь" },
  { icon: <BookOpen size={16} />, label: "Уроки" },
  { icon: <ClipboardList size={16} />, label: "Задания", active: true },
  { icon: <Trophy size={16} />, label: "Достижения" },
  { icon: <User size={16} />, label: "Профиль" },
];

function Sidebar() {
  return (
    <aside className={s.sidebar}>
      <div className={s.sidebarTop}>
        <div className={s.avatar} />
        <span className={s.brand}>Orbita Kids</span>
      </div>
      <nav className={s.nav}>
        {NAV.map(item => (
          <button
            key={item.label}
            type="button"
            className={`${s.navItem} ${item.active ? s.navItemActive : ""}`}
          >
            {item.icon}
            {item.label}
          </button>
        ))}
      </nav>
    </aside>
  );
}

// ─── Theory Phase ─────────────────────────────────────────────────────────────

function TheoryPhase({ onFinish }: { onFinish: () => void }) {
  const [idx, setIdx] = useState(0);
  const slide = THEORY_SLIDES[idx];
  const isLast = idx === THEORY_SLIDES.length - 1;

  return (
    <div className={s.theoryShell}>
      <div className={s.theoryProgress}>
        <div
          className={s.theoryProgressFill}
          style={{ width: `${((idx + 1) / THEORY_SLIDES.length) * 100}%` }}
        />
      </div>

      <div className={s.theoryBody}>
        <div className={s.theoryCard} key={idx}>
          <div className={s.theoryCardIcon}>{slide.icon}</div>
          <p className={s.theoryCardStep}>Шаг {idx + 1} из {THEORY_SLIDES.length}</p>
          <h2 className={s.theoryCardTitle}>{slide.title}</h2>
          <p className={s.theoryCardText}>{slide.text}</p>
          {"visual" in slide && slide.visual && (
            <div className={s.theoryVisualWrap}>
              <TheoryVisual visual={slide.visual} />
            </div>
          )}
        </div>
      </div>

      <div className={s.theoryNav}>
        <button
          type="button"
          className={`${s.btnNav} ${s.btnNavBack}`}
          onClick={() => setIdx(i => Math.max(0, i - 1))}
          style={{ visibility: idx === 0 ? "hidden" : "visible" }}
        >
          <ChevronLeft size={15} /> Назад
        </button>

        <div className={s.theoryDots}>
          {THEORY_SLIDES.map((_, i) => (
            <div key={i} className={`${s.theoryDot} ${i === idx ? s.theoryDotActive : ""}`} />
          ))}
        </div>

        {isLast
          ? (
              <button type="button" className={`${s.btnNav} ${s.btnNavStart}`} onClick={onFinish}>
                Начать задание 🚀
              </button>
            )
          : (
              <button
                type="button"
                className={`${s.btnNav} ${s.btnNavNext}`}
                onClick={() => setIdx(i => i + 1)}
              >
                Далее <ChevronRight size={15} />
              </button>
            )}
      </div>
    </div>
  );
}

// ─── Flowchart Task ───────────────────────────────────────────────────────────

function FlowchartTask() {
  const [workspace, setWorkspace] = useState<FlowBlock[]>([]);
  const [dragId, setDragId] = useState<string | null>(null);
  const [dragFrom, setDragFrom] = useState<"palette" | "workspace" | null>(null);
  const [hoverZone, setHoverZone] = useState<number | null>(null);
  const [result, setResult] = useState<"idle" | "correct" | "wrong">("idle");

  const inWs = (id: string) => workspace.some(b => b.id === id);
  const available = PALETTE_BLOCKS.filter(b => !inWs(b.id));

  const onPaletteDragStart = (e: React.DragEvent<HTMLDivElement>, block: FlowBlock) => {
    setDragId(block.id);
    setDragFrom("palette");
    e.dataTransfer.effectAllowed = "copy";
  };

  const onWorkspaceDragStart = (e: React.DragEvent<HTMLDivElement>, block: FlowBlock) => {
    setDragId(block.id);
    setDragFrom("workspace");
    e.dataTransfer.effectAllowed = "move";
  };

  const onDragEnd = () => {
    setDragId(null);
    setDragFrom(null);
    setHoverZone(null);
  };

  const onZoneOver = (e: React.DragEvent<HTMLDivElement>, idx: number) => {
    e.preventDefault();
    setHoverZone(idx);
  };

  const onZoneLeave = () => setHoverZone(null);

  const onZoneDrop = (e: React.DragEvent<HTMLDivElement>, zoneIdx: number) => {
    e.preventDefault();
    if (!dragId) return;
    const block = ALL_BLOCKS.find(b => b.id === dragId)!;

    if (dragFrom === "palette") {
      if (inWs(dragId)) return;
      setWorkspace(prev => {
        const next = [...prev];
        next.splice(zoneIdx, 0, block);
        return next;
      });
    } else {
      setWorkspace(prev => {
        const fromIdx = prev.findIndex(b => b.id === dragId);
        if (fromIdx === -1) return prev;
        const next = [...prev];
        next.splice(fromIdx, 1);
        const insertAt = fromIdx < zoneIdx ? zoneIdx - 1 : zoneIdx;
        next.splice(insertAt, 0, block);
        return next;
      });
    }

    setDragId(null);
    setDragFrom(null);
    setHoverZone(null);
    setResult("idle");
  };

  const onPaletteDrop = (e: React.DragEvent<HTMLDivElement>) => {
    e.preventDefault();
    if (dragFrom !== "workspace" || !dragId) return;
    setWorkspace(prev => prev.filter(b => b.id !== dragId));
    setDragId(null);
    setDragFrom(null);
    setResult("idle");
  };

  const removeFromWs = (id: string) => {
    setWorkspace(prev => prev.filter(b => b.id !== id));
    setResult("idle");
  };

  const check = () => {
    if (workspace.length !== CORRECT_ORDER.length) { setResult("wrong"); return; }
    const ok = CORRECT_ORDER.every((id, i) => workspace[i].id === id);
    setResult(ok ? "correct" : "wrong");
  };

  const reset = () => {
    setWorkspace([]);
    setResult("idle");
  };

  const showZones = !!dragId;

  return (
    <div className={s.taskShell}>
      <div className={s.taskHeader}>
        <h1 className={s.taskTitle}>📊 Блок-схема: алгоритм умножения числа на 2</h1>
        <p className={s.taskDesc}>
          Перетащи блоки из палитры в рабочую область. Чтобы убрать блок — перетащи обратно в
          палитру или нажми ×. Когда будешь готов — нажми «Проверить».
        </p>
      </div>

      <div className={s.taskBody}>
        {/* Palette */}
        <div
          className={s.palette}
          onDragOver={e => e.preventDefault()}
          onDrop={onPaletteDrop}
        >
          <h3 className={s.panelTitle}>Палитра блоков</h3>
          <div className={s.paletteList}>
            {available.map(block => (
              <div
                key={block.id}
                className={`${s.flowBlock} ${s[`shape_${block.shape}`]} ${s[`color_${block.color}`]}`}
                draggable
                onDragStart={e => onPaletteDragStart(e, block)}
                onDragEnd={onDragEnd}
                style={{
                  opacity: dragId === block.id && dragFrom === "palette" ? 0.35 : 1,
                  cursor: "grab",
                }}
              >
                <span>{block.label}</span>
              </div>
            ))}
            {available.length === 0 && (
              <p className={s.emptyMsg}>Все блоки добавлены!</p>
            )}
          </div>

          <div className={s.shapeLegend}>
            <h4 className={s.legendTitle}>Формы блоков</h4>
            <div className={s.legendRow}>
              <div className={`${s.legendIcon} ${s.legendIconOval}`} />
              <span>Начало / Конец</span>
            </div>
            <div className={s.legendRow}>
              <div className={`${s.legendIcon} ${s.legendIconRect}`} />
              <span>Действие</span>
            </div>
            <div className={s.legendRow}>
              <div className={`${s.legendIcon} ${s.legendIconPara}`} />
              <span>Ввод / Вывод</span>
            </div>
          </div>
        </div>

        {/* Workspace */}
        <div className={s.workspace}>
          <h3 className={s.panelTitle}>Твоя блок-схема</h3>

          <div className={s.flow}>
            <DropZone
              idx={0}
              active={hoverZone === 0}
              show={showZones}
              onOver={onZoneOver}
              onLeave={onZoneLeave}
              onDrop={onZoneDrop}
            />

            {workspace.length === 0 && !showZones && (
              <div className={s.flowEmpty}>
                <div className={s.flowEmptyIcon}>📦</div>
                <p>Перетащи блоки сюда из палитры</p>
              </div>
            )}

            {workspace.map((block, i) => (
              <div key={block.id} className={s.wsBlockWrap}>
                <div className={s.wsBlockRow}>
                  <div
                    className={`${s.flowBlock} ${s[`shape_${block.shape}`]} ${s[`color_${block.color}`]} ${s.wsBlock} ${dragId === block.id && dragFrom === "workspace" ? s.blockDragging : ""}`}
                    draggable
                    onDragStart={e => onWorkspaceDragStart(e, block)}
                    onDragEnd={onDragEnd}
                    style={{ cursor: "grab" }}
                  >
                    <span>{block.label}</span>
                  </div>
                  <button
                    type="button"
                    className={s.removeBtn}
                    onClick={() => removeFromWs(block.id)}
                    title="Убрать"
                  >
                    ×
                  </button>
                </div>

                {i < workspace.length - 1 && <div className={s.connArrow} />}

                <DropZone
                  idx={i + 1}
                  active={hoverZone === i + 1}
                  show={showZones}
                  onOver={onZoneOver}
                  onLeave={onZoneLeave}
                  onDrop={onZoneDrop}
                />
              </div>
            ))}
          </div>

          <div className={s.wsFooter}>
            {result === "correct" && (
              <div className={s.successBanner}>✅ Отлично! Блок-схема составлена правильно! +10 XP</div>
            )}
            {result === "wrong" && (
              <div className={s.errorBanner}>❌ Порядок неверный. Проверь последовательность и попробуй снова!</div>
            )}
            <div className={s.footerBtns}>
              <button type="button" className={s.btnReset} onClick={reset}>
                <RotateCcw size={14} /> Сбросить
              </button>
              <button
                type="button"
                className={s.btnCheck}
                onClick={check}
                disabled={workspace.length === 0}
              >
                <CheckCheck size={14} /> Проверить
              </button>
            </div>
          </div>
        </div>
      </div>
    </div>
  );
}

// ─── Root ──────────────────────────────────────────────────────────────────────

export function FlowchartChallenge() {
  const [phase, setPhase] = useState<"theory" | "task">("theory");

  useEffect(() => {
    const prevClass = document.body.className;
    document.body.className = "";
    document.body.style.background = "#f5f5f7";
    return () => {
      document.body.className = prevClass;
      document.body.style.background = "";
    };
  }, []);

  return (
    <div className={s.shell}>
      <Sidebar />
      <main className={s.main}>
        {phase === "theory"
          ? <TheoryPhase onFinish={() => setPhase("task")} />
          : <FlowchartTask />}
      </main>
    </div>
  );
}
