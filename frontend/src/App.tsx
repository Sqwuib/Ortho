import { useEffect, useState } from "react";

type Project = {
  id: number;
  name: string;
  createdAt?: string; // optional, depending on what your API returns
};

export default function App() {
  const [projects, setProjects] = useState<Project[]>([]);
  const [error, setError] = useState<string>("");

  useEffect(() => {
    fetch("/api/projects")
      .then(async (res) => {
        if (!res.ok) throw new Error(`HTTP ${res.status}`);
        return (await res.json()) as Project[];
      })
      .then(setProjects)
      .catch((e) => setError(String(e)));
  }, []);

  return (
    <div style={{ padding: 40, fontFamily: "Arial" }}>
      <h1>Ortho Projects</h1>

      {error && <p style={{ color: "crimson" }}>Error: {error}</p>}

      {!error && projects.length === 0 && <p>Loading...</p>}

      <ul>
        {projects.map((p) => (
          <li key={p.id}>
            {p.name}
            {p.createdAt ? ` (${new Date(p.createdAt).toLocaleString()})` : ""}
          </li>
        ))}
      </ul>
    </div>
  );
}