<template>
  <main class="container">
    <h1>Investments (User 1)</h1>

    <section v-if="loading">
      <p>Loading…</p>
    </section>

    <section v-else-if="error">
      <p style="color: red;">{{ error }}</p>
    </section>

    <section v-else>
      <ul v-if="investments.length > 0">
        <li v-for="inv in investments" :key="inv.investmentId">
          {{ inv.name }} (ID: {{ inv.investmentId }})
        </li>
      </ul>
      <p v-else>No investments found.</p>
    </section>
  </main>
</template>

<script setup lang="ts">
  import { onMounted, ref } from "vue";

  type InvestmentListItemDto = {
    investmentId: number;
    name: string;
  };

  const investments = ref < InvestmentListItemDto[] > ([]);
  const loading = ref < boolean > (true);
  const error = ref < string > ("");

  // For this smoke test we assume the API is reachable at the same origin under /api.
  // If your API runs on a different port, set up a Vite dev proxy or enable CORS on the API.
  const userId = 1;

  async function fetchInvestments(): Promise<void> {
    loading.value = true;
    error.value = "";

    try {
      const resp = await fetch(`/api/users/${userId}/investments`, { method: "GET" });

      if (!resp.ok) {
        throw new Error(`HTTP ${resp.status}`);
      }

      const data = (await resp.json()) as InvestmentListItemDto[];
      investments.value = Array.isArray(data) ? data : [];
    }
    catch (e: unknown) {
      error.value = e instanceof Error ? e.message : "Unknown error";
    }
    finally {
      loading.value = false;
    }
  }

  onMounted(async () => {
    await fetchInvestments();
  });
</script>

<style>
  .container {
    max-width: 720px;
    margin: 2rem auto;
    font-family: system-ui, -apple-system, Segoe UI, Roboto, Helvetica, Arial, sans-serif;
    line-height: 1.5;
  }
</style>
