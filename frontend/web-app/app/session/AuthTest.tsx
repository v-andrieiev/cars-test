"use client";
import React, { useState } from "react";
import { updateAuctionTest } from "../actions/auctionActions";
import { Button } from "flowbite-react";

export default function AuthTest() {
  const [loading, setLoading] = useState(false);
  const [result, setResult] = useState<any>();

  function doUpdate() {
    setResult(undefined);
    setLoading(true);
    updateAuctionTest()
      .then((res) => setResult(res))
      .catch((err) => setResult(err))
      .finally(() => setLoading(false));
  }
  return (
    <div>
      <Button outline isProcessing={loading} onClick={doUpdate}>
        Test auth
      </Button>
      <br />
      <div className="bg-blue-200 border-2 border-blue-500">
        {result ? (
          <pre>{JSON.stringify(result, null, 2)}</pre>
        ) : (
          <pre>No data</pre>
        )}
      </div>
    </div>
  );
}
