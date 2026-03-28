import { createFileRoute, Link, useNavigate } from "@tanstack/react-router";
import { ArrowLeftIcon } from "lucide-react";
import { useState } from "react";
import { useCreateTopic, useForumCategories } from "@/hooks/useForum";
import styles from "./create.module.css";

const RouteComponent = () => {
  const navigate = useNavigate();
  const { data: categories } = useForumCategories();
  const createTopic = useCreateTopic();

  const [title, setTitle] = useState("");
  const [content, setContent] = useState("");
  const [categoryId, setCategoryId] = useState<number | "">("");

  const canSubmit = title.trim() && content.trim() && categoryId !== "";

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    if (!canSubmit) return;

    createTopic.mutate(
      { title: title.trim(), content: content.trim(), categoryId: Number(categoryId) },
      {
        onSuccess: (topic) => {
          navigate({ to: "/forum/$topicId", params: { topicId: String(topic.id) } });
        },
      },
    );
  };

  return (
    <main className={styles.root}>
      <Link to="/forum" className={styles.backLink}>
        <ArrowLeftIcon size={16} />
        Назад к форуму
      </Link>

      <h1 className={styles.title}>Новый топик</h1>

      <form className={styles.form} onSubmit={handleSubmit}>
        <div className={styles.field}>
          <label className={styles.label} htmlFor="topic-category">
            Категория
          </label>
          <select
            id="topic-category"
            className={styles.select}
            value={categoryId}
            onChange={(e) => setCategoryId(e.target.value ? Number(e.target.value) : "")}
          >
            <option value="">Выберите категорию</option>
            {categories?.map((cat) => (
              <option key={cat.id} value={cat.id}>
                {cat.name}
              </option>
            ))}
          </select>
        </div>

        <div className={styles.field}>
          <label className={styles.label} htmlFor="topic-title">
            Заголовок
          </label>
          <input
            id="topic-title"
            className={styles.input}
            type="text"
            placeholder="О чём ваш вопрос?"
            value={title}
            onChange={(e) => setTitle(e.target.value)}
          />
        </div>

        <div className={styles.field}>
          <label className={styles.label} htmlFor="topic-content">
            Описание
          </label>
          <textarea
            id="topic-content"
            className={styles.textarea}
            placeholder="Опишите подробнее..."
            value={content}
            onChange={(e) => setContent(e.target.value)}
          />
        </div>

        {createTopic.isError && (
          <p className={styles.errorText}>Ошибка при создании топика. Попробуйте снова.</p>
        )}

        <div className={styles.actions}>
          <button
            type="submit"
            className={styles.submitBtn}
            disabled={!canSubmit || createTopic.isPending}
          >
            {createTopic.isPending ? "Создание..." : "Создать топик"}
          </button>
        </div>
      </form>
    </main>
  );
};

export const Route = createFileRoute("/_protected/_sidebar/forum/create")({
  component: RouteComponent,
});
