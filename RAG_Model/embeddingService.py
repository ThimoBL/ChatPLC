from qdrant_client import QdrantClient, models
from voyageai import Client
import os


class EmbeddingService:
    def __init__(self, qdrant_client: QdrantClient, voyage_client: Client):
        self.qdrant_client = qdrant_client
        self.voyage_client = voyage_client

        # Check if Qdrant collection exists otherwise create it
        if not self.qdrant_client.collection_exists(collection_name="text_collection"):
            self.qdrant_client.create_collection(
                collection_name="text_collection",
                vectors_config=models.VectorParams(
                    size=1024,  # Size of the embeddings
                    distance=models.Distance.COSINE,  # Distance metric
                )
            )

    # Embed question/query using voyage-code-3:
    def embed_query(self, query: str):
        # Convert query into vector
        query_embed = self.voyage_client.embed([query], model="voyage-code-3", input_type="query").embeddings[0]

        # return the vector
        return query_embed

    # Embed document using voyage-code-3:
    def embed_document(self, document: str):
        # Convert document into vector
        doc_embed = self.voyage_client.embed([document], model="voyage-code-3", input_type="document").embeddings[0]

        # Upload data to collection
        # ToDo: refactor this so it wont treat document as array
        self.qdrant_client.upload_points(
            collection_name="text_collection",
            points=[
                models.PointStruct(
                    id=idx,
                    vector=doc_embed,
                    payload=document
                )
                for idx, document in enumerate([document])
            ],
        )

    # Retrieve document using voyage-code-3 from the vector store:
    def retrieve_document(self, query: str):
        # Convert query into vector
        query_embed = self.voyage_client.embed([query], model="voyage-code-3", input_type="query").embeddings[0]
        print(query_embed)
        # Check vector store for similar documents using cosine similarity
        search_result = self.qdrant_client.query_points(
            collection_name="text_collection",
            query=query_embed,
            limit=2,  # Return the top 3 most similar documents
            with_payload=True,  # Include the payload in the result
            score_threshold=0.6,  # Set a score threshold for filtering results
            ).points

        # Return the most similar 3 documents
        return search_result

    def initial_run(self):
        documents = []

        # Check if the directory exists
        directory = os.fsencode("code_corpus")

        # Iterate through the files in the directory
        for document in os.listdir(directory):
            filename = os.fsdecode(document)
            if filename.endswith(".txt"):
                file_path = os.path.join("code_corpus", filename)
                try:
                    # Open the file and read its contents
                    with open(file_path, 'r') as file:
                        content = file.read()
                        documents.append(content)
                except Exception as e:
                    print(f"Error reading file {filename}: {e}")
                    continue

        # Check if the collection is empty
        collection_len = self.qdrant_client.count(collection_name="text_collection")

        # If the collection is empty or the number of documents in the collection is not equal to the number of documents in the directory,
        # upload the documents
        if collection_len == 0 or not collection_len == len(documents):
            print("Uploading documents to the vector store...")

            # Batch embed documents
            embedded_documents = self.voyage_client.embed(documents, model="voyage-code-3",
                                                          input_type="document").embeddings

            # Upload data to collection in vector store
            self.qdrant_client.upload_collection(
                collection_name="text_collection",
                vectors=[
                    embedded_documents[0],
                    embedded_documents[1],
                    embedded_documents[2],
                    embedded_documents[3],
                    embedded_documents[4],
                    embedded_documents[5],
                ],
                payload=[
                    {"document": documents[0]},
                    {"document": documents[1]},
                    {"document": documents[2]},
                    {"document": documents[3]},
                    {"document": documents[4]},
                    {"document": documents[5]},
                ],
                parallel=2,
                max_retries=3
            )

            return "Documents uploaded successfully."
        else:
            return "Documents already exist in the vector store. No need to upload."
