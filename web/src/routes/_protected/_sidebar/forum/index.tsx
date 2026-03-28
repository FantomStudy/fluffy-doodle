import type { ForumCategory } from "@/api/forum";
import { createFileRoute, Link, useNavigate } from "@tanstack/react-router";
import { CheckCircle2Icon, EyeIcon, MessageSquareIcon, PlusIcon } from "lucide-react";
import { useState } from "react";
import {
  Dialog,
  DialogContent,
  DialogHeader,
  DialogTitle,
  DialogTrigger,
} from "@/components/ui/Dialog";
import {
  useCreateCategory,
  useCreateTopic,
  useForumCategories,
  useForumTopics,
} from "@/hooks/useForum";
import styles from "./index.module.css";

const formatDate = (iso: string) => {
  const d = new Date(iso);
  return d.toLocaleDateString("ru-RU", { day: "numeric", month: "short", year: "numeric" });
};

const CategoryCard = ({
  category,
  isActive,
  onClick,
}: {
  category: ForumCategory;
  isActive: boolean;
  onClick: () => void;
}) => (
  <button
    className={`${styles.categoryCard} ${isActive ? styles.categoryCardActive : ""}`}
    onClick={onClick}
    type="button"
  >
    <span className={styles.categoryName}>{category.name}</span>
    <span className={styles.categoryMeta}>{category.topicsCount}</span>
  </button>
);

const CreateTopicDialog = ({ categories }: { categories: ForumCategory[] | null | undefined }) => {
  const navigate = useNavigate();
  const createTopic = useCreateTopic();
  const createCategory = useCreateCategory();
  const [open, setOpen] = useState(false);
  const [title, setTitle] = useState("");
  const [content, setContent] = useState("");
  const [categoryId, setCategoryId] = useState<number | "">("");
  const [showNewCat, setShowNewCat] = useState(false);
  const [newCatName, setNewCatName] = useState("");
  const [newCatDesc, setNewCatDesc] = useState("");

  const hasCategories = categories && categories.length > 0;
  const canSubmit = title.trim() && content.trim() && categoryId !== "";

  const handleCreateCategory = () => {
    if (!newCatName.trim()) return;

    createCategory.mutate(
      {
        name: newCatName.trim(),
        description: newCatDesc.trim(),
        order: (categories?.length ?? 0) + 1,
      },
      {
        onSuccess: (cat) => {
          setCategoryId(cat.id);
          setNewCatName("");
          setNewCatDesc("");
          setShowNewCat(false);
        },
      },
    );
  };

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    if (!canSubmit) return;

    createTopic.mutate(
      { title: title.trim(), content: content.trim(), categoryId: Number(categoryId) },
      {
        onSuccess: (topic) => {
          setOpen(false);
          setTitle("");
          setContent("");
          setCategoryId("");
          navigate({ to: "/forum/$topicId", params: { topicId: String(topic.id) } });
        },
      },
    );
  };

  return (
    <Dialog open={open} onOpenChange={setOpen}>
      <DialogTrigger className={styles.createBtn}>
        <PlusIcon size={16} />
        Новый топик
      </DialogTrigger>
      <DialogContent>
        <DialogHeader>
          <DialogTitle>Новый топик</DialogTitle>
        </DialogHeader>
        <form className={styles.dialogForm} onSubmit={handleSubmit}>
          <div className={styles.field}>
            <label className={styles.fieldLabel} htmlFor="topic-category">
              Категория
            </label>
            {hasCategories && !showNewCat ? (
              <div className={styles.categorySelectRow}>
                <select
                  id="topic-category"
                  className={styles.fieldSelect}
                  value={categoryId}
                  onChange={(e) => setCategoryId(e.target.value ? Number(e.target.value) : "")}
                >
                  <option value="">Выберите категорию</option>
                  {categories.map((cat) => (
                    <option key={cat.id} value={cat.id}>
                      {cat.name}
                    </option>
                  ))}
                </select>
                <button
                  type="button"
                  className={styles.newCatBtn}
                  onClick={() => setShowNewCat(true)}
                >
                  <PlusIcon size={14} />
                  Новая
                </button>
              </div>
            ) : (
              <div className={styles.newCatForm}>
                <input
                  className={styles.fieldInput}
                  type="text"
                  placeholder="Название категории"
                  value={newCatName}
                  onChange={(e) => setNewCatName(e.target.value)}
                />
                <input
                  className={styles.fieldInput}
                  type="text"
                  placeholder="Описание (необязательно)"
                  value={newCatDesc}
                  onChange={(e) => setNewCatDesc(e.target.value)}
                />
                <div className={styles.newCatActions}>
                  <button
                    type="button"
                    className={styles.submitBtn}
                    disabled={!newCatName.trim() || createCategory.isPending}
                    onClick={handleCreateCategory}
                  >
                    {createCategory.isPending ? "Создание..." : "Создать категорию"}
                  </button>
                  {hasCategories && (
                    <button
                      type="button"
                      className={styles.newCatBtn}
                      onClick={() => setShowNewCat(false)}
                    >
                      Отмена
                    </button>
                  )}
                </div>
                {createCategory.isError && (
                  <p className={styles.dialogError}>Ошибка при создании категории.</p>
                )}
              </div>
            )}
          </div>

          <div className={styles.field}>
            <label className={styles.fieldLabel} htmlFor="topic-title">
              Заголовок
            </label>
            <input
              id="topic-title"
              className={styles.fieldInput}
              type="text"
              placeholder="О чём ваш вопрос?"
              value={title}
              onChange={(e) => setTitle(e.target.value)}
            />
          </div>

          <div className={styles.field}>
            <label className={styles.fieldLabel} htmlFor="topic-content">
              Описание
            </label>
            <textarea
              id="topic-content"
              className={styles.fieldTextarea}
              placeholder="Опишите подробнее..."
              value={content}
              onChange={(e) => setContent(e.target.value)}
            />
          </div>

          {createTopic.isError && (
            <p className={styles.dialogError}>Ошибка при создании. Попробуйте снова.</p>
          )}

          <div className={styles.dialogActions}>
            <button
              type="submit"
              className={styles.submitBtn}
              disabled={!canSubmit || createTopic.isPending}
            >
              {createTopic.isPending ? "Создание..." : "Создать топик"}
            </button>
          </div>
        </form>
      </DialogContent>
    </Dialog>
  );
};

const RouteComponent = () => {
  const [selectedCategoryId, setSelectedCategoryId] = useState<number | undefined>(undefined);
  const { data: categories, isLoading: loadingCats } = useForumCategories();
  const { data: topics, isLoading: loadingTopics } = useForumTopics(selectedCategoryId);

  const selectedCategory = categories?.find((c) => c.id === selectedCategoryId);

  if (loadingCats) {
    return (
      <main className={styles.root}>
        <p>Загрузка...</p>
      </main>
    );
  }

  return (
    <main className={styles.root}>
      <div className={styles.header}>
        <h1 className={styles.title}>Форум</h1>
        <p className={styles.subtitle}>Обсуждай, задавай вопросы, помогай другим</p>
      </div>

      <div className={styles.categories}>
        {categories?.map((cat) => (
          <CategoryCard
            key={cat.id}
            category={cat}
            isActive={cat.id === selectedCategoryId}
            onClick={() =>
              setSelectedCategoryId(cat.id === selectedCategoryId ? undefined : cat.id)
            }
          />
        ))}
      </div>

      <div className={styles.topicsHeader}>
        <div>
          {selectedCategory ? (
            <button
              type="button"
              className={styles.backBtn}
              onClick={() => setSelectedCategoryId(undefined)}
            >
              ← Все категории
            </button>
          ) : null}
        </div>
        <CreateTopicDialog categories={categories} />
      </div>

      {loadingTopics ? (
        <p>Загрузка топиков...</p>
      ) : topics && topics.length > 0 ? (
        <div className={styles.topicsList}>
          {topics.map((topic) => (
            <Link
              key={topic.id}
              to="/forum/$topicId"
              params={{ topicId: String(topic.id) }}
              className={styles.topicRow}
            >
              <div className={styles.topicInfo}>
                <div className={styles.topicTitle}>
                  {topic.isSolved && <CheckCircle2Icon size={16} className={styles.solvedIcon} />}
                  {topic.title}
                </div>
                <p className={styles.topicExcerpt}>{topic.content}</p>
                <div className={styles.topicMeta}>
                  <div className={styles.authorRow}>
                    {topic.author.avatar ? (
                      <img src={topic.author.avatar} alt="" className={styles.avatarSmall} />
                    ) : (
                      <div className={styles.avatarSmall} />
                    )}
                    <span className={styles.authorName}>{topic.author.fullName}</span>
                  </div>
                  <span className={styles.dot} />
                  <span>{formatDate(topic.createdAt)}</span>
                </div>
              </div>
              <div className={styles.topicStats}>
                <span className={styles.stat}>
                  <MessageSquareIcon />
                  {topic.replies}
                </span>
                <span className={styles.stat}>
                  <EyeIcon />
                  {topic.views}
                </span>
              </div>
            </Link>
          ))}
        </div>
      ) : (
        <p className={styles.emptyText}>Пока нет топиков</p>
      )}
    </main>
  );
};

export const Route = createFileRoute("/_protected/_sidebar/forum/")({
  component: RouteComponent,
});
