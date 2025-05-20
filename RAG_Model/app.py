# Import necessary libraries
import json
import os
from dotenv import load_dotenv

import voyageai
from qdrant_client import QdrantClient
from flask import Flask, jsonify, request

# Import custom classes
from embeddingService import EmbeddingService

load_dotenv()

app = Flask(__name__)

# Configure Swagger UI


# Initialize Qdrant and VoyageAI clients
qdrant_client = QdrantClient(host="localhost", port=6333)
voyage_client = voyageai.Client(api_key=os.getenv("VOYAGE_API_KEY"))

# Initialize Services
embeddingService = EmbeddingService(qdrant_client = qdrant_client, voyage_client = voyage_client)

@app.route('/', methods=['GET'])
def test_api():
    try:
        result = embeddingService.initial_run()
        return jsonify({"result": result}), 200
    except Exception as e:
        print(f"Error: {e}")
        return jsonify({"error": str(e)}), 500

@app.route('/test_embed_document', methods=['POST'])
def embed_document():
    #Get data from application/json
    request_data = request.get_json()

    #get document from request_data
    document = request_data.get('document')

    #print document
    embeddingService.embed_document(document)

    #print embedded document
    print(document)

    # Return the document as a JSON response
    return jsonify({"document": document})

@app.route('/test_embed_query', methods=['POST'])
def embed_query():
    try:
        #Get data from application/json
        print(request)
        request_data = request.get_json()

        #get query from request_data
        query = request_data.get('query')

        #print embedded query
        relevant_docs = embeddingService.retrieve_document(query)

        # Return the query as a JSON response
        # return jsonify({"query": query, "relevant_docs": relevant_docs})
        return [doc.model_dump() for doc in relevant_docs]
    except Exception as e:
        print(f"Error: {e}")
        return jsonify({"error": str(e)}), 500

@app.route('/insert_document', methods=['POST'])
def insert_document():
    # Get data from application/json
    request_data = request.get_json()

    # get document from request_data
    document = request_data.get('document')

    # print document
    print(document)

if __name__ == '__main__':
    app.run(port=5000, debug=True)
