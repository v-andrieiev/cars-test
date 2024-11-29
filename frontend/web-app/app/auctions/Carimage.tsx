'use client'
import React, { useState } from "react";
import Image from "next/image";

type Props = {
  imageUrl: string;
  make: string;
  model: string;
  color: string;
};

export default function Carimage({ imageUrl, make, model, color }: Props) {
  const [isLoading, setLoading] = useState(true);
  return (
    <Image
      src={imageUrl}
      alt={`Image of ${make} ${model} in ${color}`}
      fill
      priority
      className={`object-cover group-hover:opacity-75 duration-700 ease-in-out
        ${isLoading ? 'grayscale blur-2xl scale-110' : 'grayscale-0 blur-0 scale-100'}`}
      sizes="(max-width: 768px) 100vw, (max-width: 1200px) 50vw, 25vw"
      onLoad={() => setLoading(false)}
    />
  );
}
